using Shared.Models;

namespace Consumer.Client
{
    public interface IOrdersClient: IAsyncDisposable
    {
        Task<OrderDTO> GetAsync(int orderId);
        Task<IList<OrderDTO>> GetAllAsync();
        ValueTask UpdateAsync(int orderId, OrderStatus status);
    }
}
