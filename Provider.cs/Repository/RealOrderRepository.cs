
using Provider.cs.Models;
using System.Collections.Concurrent;

namespace Provider.cs.Repository
{
    public class RealOrderRepository : IOrderRepository
    {
        private readonly ConcurrentBag<OrderDTO> _entities = new ConcurrentBag<OrderDTO>()
        {
            new OrderDTO {CreationDate= DateTime.UtcNow.AddDays(-1), Id= 1, Name="Test 1", Status = OrderStatus.Initial},
            new OrderDTO {CreationDate= DateTime.UtcNow.AddDays(-2), Id= 2, Name="Test 2", Status = OrderStatus.Initial},
            new OrderDTO {CreationDate= DateTime.UtcNow.AddDays(1), Id= 3, Name="Test 3", Status = OrderStatus.Delivered},
        };
        public Task<IList<OrderDTO>> GetAllAsync()
        {
            return Task.FromResult<IList<OrderDTO>>(_entities.ToList());
        }

        public Task<OrderDTO> GetAsync(int id)
        {
            var entity = _entities.SingleOrDefault(x=> x.Id == id);

            return Task.FromResult(entity);
        }

        public Task InsertAsync(OrderDTO orderDTO)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(int id, OrderStatus status)
        {
            var entity = _entities.SingleOrDefault(x => x.Id == id);

            entity.Status = status;

            return Task.CompletedTask;
        }
    }
}
