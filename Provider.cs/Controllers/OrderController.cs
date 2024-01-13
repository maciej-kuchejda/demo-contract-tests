using Microsoft.AspNetCore.Mvc;
using Provider.cs.Models;
using Provider.cs.Repository;

namespace Provider.cs.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrderController : ControllerBase
    {
        private IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var entity = await _orderRepository.GetAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var entities = await _orderRepository.GetAllAsync();

            return Ok(entities);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateOrderModel model)
        {
            await _orderRepository.UpdateAsync(id, model.OrderStatus);

            return Accepted();
        }
    }
}
