using Consumer.Client;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using PactNet;
using PactNet.Output.Xunit;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using PactNet.Infrastructure.Outputters;

namespace Consumer.Tests.cs
{
    public class OrderClientTests
    {
        private IPactBuilderV3 _pact;

        [SetUp]
        public void Setup()
        {

            var config = new PactConfig
            {
                PactDir = "../../../pacts/",
                Outputters = new[]
                {
                    new ConsoleOutput()
                },
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                },
                LogLevel = PactLogLevel.Debug
            };

            _pact = Pact.V3("Consumer API", "Orders API", config).WithHttpInteractions();
        }

        [Test]
        public async Task GetOrderAsync_WhenCalled_ReturnsOrder()
        {
            var expected = new OrderDTO()
            {
                CreationDate= DateTime.Now,
                Id= 1,
                Name = "Test 1",
                Status = OrderStatus.Initial
            };

            _pact
                .UponReceiving("a request for an order by ID")
                    .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "1" })
                    .WithRequest(HttpMethod.Get, "/api/orders/1")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithJsonBody(new
                    {
                        Id = PactNet.Matchers.Match.Integer(expected.Id),
                        Status = PactNet.Matchers.Match.Equality(0),
                        CreationDate = PactNet.Matchers.Match.Type(expected.CreationDate.ToString("O")),
                        Name = PactNet.Matchers.Match.Equality(expected.Name)
                    });

            await _pact.VerifyAsync(async ctx =>
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = ctx.MockServerUri
                };
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json"); 

                var client = new OrderClient(httpClient);

                OrderDTO order = await client.GetAsync(1);

                order.CreationDate.Should().Be(expected.CreationDate);
                order.Id.Should().Be(expected.Id);
                order.Name.Should().Be(expected.Name);
                order.Status.Should().Be(expected.Status);
            });
        }

        [Test]
        public async Task GetOrderAsync_UnknownOrder_ReturnsNotFound()
        {
            _pact
                .UponReceiving("a request for an order with an unknown ID")
                    .WithRequest(HttpMethod.Get, "/api/orders/404")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.NotFound);

            await _pact.VerifyAsync(async ctx =>
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = ctx.MockServerUri
                };
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var client = new OrderClient(httpClient);

                Func<Task> action = () => client.GetAsync(404);

                var response = await action.Should().ThrowAsync<HttpRequestException>();
                response.And.StatusCode.Should().Be(HttpStatusCode.NotFound);
            });
        }

        [Test]
        public async Task GetAllOrderAsync_SomeEntitiesExists_ReturnsExists()
        {
            var expected = new OrderDTO()
            {
                CreationDate = DateTime.Now,
                Id = 123,
                Name = "Test 123",
                Status = OrderStatus.Initial
            };

            _pact
                .UponReceiving("a request for all orders")
                    .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "123" })
                    .WithRequest(HttpMethod.Get, "/api/orders")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithJsonBody(new[]
                    {
                        new
                        {
                            Id = PactNet.Matchers.Match.Integer(expected.Id),
                            Status = PactNet.Matchers.Match.Equality(0),
                            CreationDate = PactNet.Matchers.Match.Type(expected.CreationDate.ToString("O")),
                            Name = PactNet.Matchers.Match.Equality(expected.Name)
                        }
                    });

            await _pact.VerifyAsync(async ctx =>
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = ctx.MockServerUri
                };
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var client = new OrderClient(httpClient);

                var orders = await client.GetAllAsync();

                var order = orders[0];

                order.CreationDate.Should().Be(expected.CreationDate);
                order.Id.Should().Be(expected.Id);
                order.Name.Should().Be(expected.Name);
                order.Status.Should().Be(expected.Status);
            });
        }

        [Test]
        public async Task UpdateOrderAsync_EntityExists_ReturnsAccepted()
        {
            _pact
               .UponReceiving("a update for an order by ID")
                   .Given("an order with ID {id} exists", new Dictionary<string, string> { ["id"] = "2" })
                   .WithRequest(HttpMethod.Put, "/api/orders/2")
                   .WithHeader("Accept", "application/json")
                   .WithJsonBody(new
                   {
                       status = OrderStatus.Delivered
                   })
               .WillRespond()
                   .WithStatus(HttpStatusCode.Accepted);


            await _pact.VerifyAsync(async ctx =>
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = ctx.MockServerUri
                };
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var client = new OrderClient(httpClient);

                await client.UpdateAsync(2, OrderStatus.Delivered);
            });
        }
    }
}
