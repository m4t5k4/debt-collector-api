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
    public class ExpensesController : ControllerBase
    {
        private readonly DebtCollectorContext _context;

        public ExpensesController(DebtCollectorContext context)
        {
            _context = context;
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByGroup(int groupId)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var isUserInGroup = await _context.PersonGroup
                .AnyAsync(gu => gu.GroupId == groupId && gu.PersonId == userId);

            if (!isUserInGroup)
            {
                return Forbid("You are not a member of this group.");
            }

            var expenses = await _context.Expense
                .Where(e => e.GroupId == groupId)
                .ToListAsync();

            return Ok(expenses);
        }

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expense.FindAsync(id);

            if (expense == null)
            {
                return NotFound();
            }

            return expense;
        }

        // PUT: api/Expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExpense(int id, Expense expense)
        {
            if (id != expense.Id)
            {
                return BadRequest();
            }

            _context.Entry(expense).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(CreateExpenseRequest expenseDto)
        {
            var personId = GetCurrentUserId();

            if (personId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var group = await _context.Group.FindAsync(expenseDto.GroupId);
            if (group == null)
            {
                return BadRequest(new { message = "Invalid GroupId, group does not exist." });
            }

            var isUserInGroup = await _context.PersonGroup
                .AnyAsync(gu => gu.GroupId == expenseDto.GroupId && gu.PersonId == personId);

            if (!isUserInGroup)
            {
                return Forbid("You are not a member of this group.");
            }

            var expense = new Expense
            {
                GroupId = expenseDto.GroupId,
                Name = expenseDto.Name,
                Currency = expenseDto.Currency,
                CreatedByPersonId = personId.Value,
                ModifiedByPersonId = personId.Value,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow
            };

            _context.Expense.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
        }

        // DELETE: api/Expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expense.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expense.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expense.Any(e => e.Id == id);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }

            return null;
        }
    }
}
