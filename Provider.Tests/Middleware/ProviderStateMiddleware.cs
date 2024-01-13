using Microsoft.AspNetCore.Http;
using PactNet;
using Provider.cs.Models;
using Provider.cs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Provider.Tests.Middleware
{
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IDictionary<string, Func<IDictionary<string, object>, Task>> _providerStates;
        private readonly RequestDelegate _next;
        private readonly IOrderRepository _orderRepository;

        public ProviderStateMiddleware(RequestDelegate next, IOrderRepository orderRepository)
        {
            _next = next;
            _orderRepository = orderRepository;

            _providerStates = new Dictionary<string, Func<IDictionary<string, object>, Task>>
            {
                ["an order with ID {id} exists"] = EnsureOrderExistsAsync
            };
        }

        private async Task EnsureOrderExistsAsync(IDictionary<string, object> parameters)
        {
            JsonElement id = (JsonElement)parameters["id"];
            await _orderRepository.InsertAsync(new OrderDTO()
            {
                CreationDate= DateTime.Now,
                Id = id.GetInt32(),
                Status = OrderStatus.Initial,
                Name = $"Test {id}" 
            });
        }

        /// <summary>
        /// Handle the request
        /// </summary>
        /// <param name="context">Request context</param>
        /// <returns>Awaitable</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await _next.Invoke(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;

            if (context.Request.Method == HttpMethod.Post.ToString())
            {
                string jsonRequestBody;

                try
                {
                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                        jsonRequestBody = await reader.ReadToEndAsync();
                    }

                    ProviderState providerState = JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);

                    if (!string.IsNullOrEmpty(providerState?.State))
                    {
                        await _providerStates[providerState.State].Invoke(providerState.Params);
                    }
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Failed to deserialise JSON provider state body:");
                    //await context.Response.WriteAsync(jsonRequestBody);
                    await context.Response.WriteAsync(string.Empty);
                    await context.Response.WriteAsync(e.ToString());
                }
            }
        }

        public record ProviderState(string State, IDictionary<string, object> Params);
    }
}
