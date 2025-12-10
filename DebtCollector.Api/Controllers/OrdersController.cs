using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Infrastructure.Persistence;
using DebtCollector.Domain.Entities;
using DebtCollector.Application.Orders.Commands.CreateOrder;
using DebtCollector.Application.Orders.Commands.UpdateOrder;
using DebtCollector.Application.Orders.Commands.DeleteOrder;
using DebtCollector.Application.Orders.Queries.GetOrders;
using DebtCollector.Application.Orders.Queries.GetOrderById;
using MediatR;
using DebtCollector.Application.DTOs;

namespace DebtCollector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _sender;

        public OrdersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return Ok(await _sender.Send(new GetOrdersQuery()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            try
            {
                return Ok(await _sender.Send(new GetOrderByIdQuery { Id = id }));
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(CreateOrderCommand command)
        {
            try
            {
                var order = await _sender.Send(command);
                return CreatedAtAction("GetOrder", new { id = order.Id }, order);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderDTO updateOrderDto)
        {
            try
            {
                 var result = await _sender.Send(new UpdateOrderCommand { Id = orderId, OrderDto = updateOrderDto });
                 return Ok(result);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
             try
            {
                await _sender.Send(new DeleteOrderCommand { Id = id });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
        }
    }
}
