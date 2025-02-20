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

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly DebtCollectorContext _context;

        public OrdersController(DebtCollectorContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return await _context.Orders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Payers)
                .Include(o => o.Debtors)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(CreateOrderRequest createOrderRequest)
        {
            var personId = GetCurrentPersonId();

            if (personId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var expense = await _context.Expenses
                .Include(e => e.Group)
                .Where(e => e.Id == createOrderRequest.ExpenseId)
                .FirstOrDefaultAsync();

            if (expense == null)
            {
                return NotFound(new { message = "Expense not found" });
            }

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId);

            if (!isPersonInGroup)
            {
                return Forbid();
            }

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private int? GetCurrentPersonId()
        {
            var nameIdentifierClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (nameIdentifierClaim != null)
            {
                return int.Parse(nameIdentifierClaim.Value);
            }

            return null;
        }
    }
}
