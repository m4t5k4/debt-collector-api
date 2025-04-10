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
    public class ExpensesController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;

        public ExpensesController(
            DebtCollectorContext context,
            AuthorizationHelper authorizationHelper
            )
        {
            _context = context;
            _authorizationHelper = authorizationHelper;
        }

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var expense = await _context.Expenses
                .Include(e => e.Orders)
                .Include(e => e.Image)
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (expense == null) return NotFound(new { message = "Expense not found" });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == expense.GroupId);

            if (!isPersonInGroup) return Forbid();

            return Ok(expense);
        }

        // POST: api/Expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(CreateExpenseRequest expenseDto)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var group = await _context.Groups.FindAsync(expenseDto.GroupId);

            if (group == null) return BadRequest(new { message = "Invalid GroupId, group does not exist." });

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(gu => gu.GroupId == expenseDto.GroupId && gu.PersonId == personId);

            if (!isPersonInGroup) return Forbid("You are not a member of this group.");

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

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
        }

        [HttpPut("{expenseId}")]
        public async Task<IActionResult> UpdateExpense(int expenseId, [FromBody] ExpenseDTO updatedExpenseDto)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (expense == null) return NotFound(new { message = "Expense not found" });

            expense.Name = updatedExpenseDto.Name;
            expense.ModifiedOn = DateTime.UtcNow;
            expense.ModifiedByPersonId = personId.Value;

            await _context.SaveChangesAsync();

            return Ok(expense);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
