using Shared.Models;

namespace Consumer.Repository
{
    public interface IOrderRepository : IAsyncDisposable
    {
        Task<OrderDTO> GetAsync(int id);

        Task UpdateAsync(int id, OrderStatus status);

        Task<IList<OrderDTO>> GetAllAsync();
    }
}
