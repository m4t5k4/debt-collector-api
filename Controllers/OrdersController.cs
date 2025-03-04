using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using debt_collector_api.Data;
using debt_collector_api.Models;
using System.Security.Claims;
using debt_collector_api.Requests;
using debt_collector_api.Helpers;
using debt_collector_api.Responses;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;

        public OrdersController(
            DebtCollectorContext context,
            AuthorizationHelper authorizationHelper
            )
        {
            _context = context;
            _authorizationHelper = authorizationHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var order = await _context.Orders
                .Include(o => o.Payers)
                .Include(o => o.Debtors)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound(new { message = "Order not found" });

            var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == order.ExpenseId);

            if (expense == null) return NotFound(new { message = "Order is not part of an expense" });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId);

            if (!isPersonInGroup) return Forbid();

            return order;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(CreateOrderRequest createOrderRequest)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var expense = await _context.Expenses
                .Include(e => e.Group)
                .Where(e => e.Id == createOrderRequest.ExpenseId)
                .FirstOrDefaultAsync();

            if (expense == null) return NotFound(new { message = "Expense not found" });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId);

            if (!isPersonInGroup) return Forbid();

            Order newOrder = new()
            {
                Id = createOrderRequest.Id,
                ExpenseId = expense.Id,
                Name = createOrderRequest.Name,
                TotalCost = createOrderRequest.TotalCost,
                Debtors = [],
                Payers = [],
                CreatedOn = DateTime.UtcNow,
                CreatedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            foreach (var payer in createOrderRequest.Payers)
            {
                Payer newPayer = new()
                {
                    Id = 0,
                    PersonId = payer.PersonId,
                    OrderId = newOrder.Id,
                    Value = payer.Value,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByPersonId = personId ?? 0,
                    ModifiedOn = DateTime.UtcNow,
                    ModifiedByPersonId = personId ?? 0
                };
                _context.Payers.Add(newPayer);
            }
            foreach (var debtor in createOrderRequest.Debtors)
            {
                Debtor newDebtor = new()
                {
                    Id = 0,
                    PersonId = debtor.PersonId,
                    OrderId = newOrder.Id,
                    Value = debtor.Value,
                    CreatedOn = DateTime.UtcNow,
                    CreatedByPersonId = personId ?? 0,
                    ModifiedOn = DateTime.UtcNow,
                    ModifiedByPersonId = personId ?? 0
                };
                _context.Debtors.Add(newDebtor);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = createOrderRequest.Id }, createOrderRequest);
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrderDTO updateOrderDto)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var order = await _context.Orders
                .Include(o => o.Payers)
                .Include(o => o.Debtors)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return NotFound(new { message = "Order not found" });

            _context.Payers.RemoveRange(order.Payers);
            _context.Debtors.RemoveRange(order.Debtors);

            order.Name = updateOrderDto.Name;
            order.TotalCost = updateOrderDto.TotalCost;
            order.Payers = updateOrderDto.Payers.Select(p => new Payer
            {
                Id = 0,
                PersonId = p.PersonId,
                OrderId = p.OrderId,
                Value = p.Value,
                CreatedByPersonId = personId ?? 0,
                CreatedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow
            }).ToList();
            order.Debtors = updateOrderDto.Debtors.Select(d => new Debtor
            {
                Id = 0,
                PersonId = d.PersonId,
                OrderId = d.OrderId,
                Value = d.Value,
                HasPaid = d.HasPaid,
                CreatedByPersonId = personId ?? 0,
                CreatedOn = DateTime.UtcNow,
                ModifiedByPersonId = personId ?? 0,
                ModifiedOn = DateTime.UtcNow
            }).ToList();
            order.ModifiedOn = DateTime.UtcNow;
            order.ModifiedByPersonId = personId ?? 0;

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var order = await _context.Orders.FindAsync(id);

            if (order == null) return NotFound();

            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
