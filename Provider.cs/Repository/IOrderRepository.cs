
using Provider.cs.Models;

namespace Provider.cs.Repository
{
    public interface IOrderRepository
    {
        Task<OrderDTO> GetAsync(int id);

        Task UpdateAsync(int id, OrderStatus status);

        Task<IList<OrderDTO>> GetAllAsync();

        Task InsertAsync(OrderDTO orderDTO);
    }
}
