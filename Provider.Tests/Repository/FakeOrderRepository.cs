using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Provider.cs.Models;
using Provider.cs.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.Tests.Repository
{
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly ConcurrentBag<OrderDTO> _fakes;

        public FakeOrderRepository()
        {
            _fakes = new ConcurrentBag<OrderDTO>();
        }

        public Task<IList<OrderDTO>> GetAllAsync()
        {
            return Task.FromResult<IList<OrderDTO>>(_fakes.ToList());
        }

        public Task<OrderDTO> GetAsync(int id)
        {
            return Task.FromResult(_fakes.SingleOrDefault(x=> x.Id == id));
        }

        public Task InsertAsync(OrderDTO orderDTO)
        {
            _fakes.Add(orderDTO);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(int id, OrderStatus status)
        {
            var fake = _fakes.SingleOrDefault(x=> x.Id ==id);
            fake.Status = status;
            return Task.CompletedTask;
        }
    }
}
