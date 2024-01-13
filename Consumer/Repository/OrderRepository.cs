using Consumer.Client;
using Shared.Models;

namespace Consumer.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrdersClient _ordersClient;
        public OrderRepository(string hostUrl)
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri(hostUrl)
            };
            _ordersClient = new OrderClient(client);
        }

        public async ValueTask DisposeAsync()
        {
            await _ordersClient.DisposeAsync();
        }

        public async Task<IList<OrderDTO>> GetAllAsync()
        {
            return await _ordersClient.GetAllAsync();
        }

        public async Task<OrderDTO> GetAsync(int id)
        {
            return await _ordersClient.GetAsync(id);
        }

        public async Task UpdateAsync(int id, OrderStatus status)
        {
            await _ordersClient.UpdateAsync(id, status);
        }
    }
}
