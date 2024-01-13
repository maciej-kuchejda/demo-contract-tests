using Shared.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Consumer.Client
{
    public class OrderClient : IOrdersClient
    {
        private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };
        private readonly HttpClient _client;

        public OrderClient(HttpClient client)
        {
            _client = client;
        }

        public ValueTask DisposeAsync()
        {
            _client.Dispose();
            return ValueTask.CompletedTask;
        }

        public async Task<IList<OrderDTO>> GetAllAsync()
        {
            var items = await _client.GetFromJsonAsync<IList<OrderDTO>>($"/api/orders", Options);
            return items;
        }

        public async Task<OrderDTO> GetAsync(int orderId)
        {
            OrderDTO order = await _client.GetFromJsonAsync<OrderDTO>($"/api/orders/{orderId}", Options);
            return order;
        }

        public async ValueTask UpdateAsync(int orderId, OrderStatus orderStatus)
        {
            await _client.PutAsJsonAsync($"/api/orders/{orderId}", new { status= (int)orderStatus }, Options);
        }
    }
}
