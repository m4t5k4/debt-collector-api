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

        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var personId = GetCurrentPersonId();

            if (personId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var expense = await _context.Expenses
                .Include(e => e.Group)
                .Where(e => e.Id == id)
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

            return Ok(expense);
        }

        // POST: api/Expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(CreateExpenseRequest expenseDto)
        {
            var personId = GetCurrentPersonId();

            if (personId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var group = await _context.Groups.FindAsync(expenseDto.GroupId);
            if (group == null)
            {
                return BadRequest(new { message = "Invalid GroupId, group does not exist." });
            }

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(gu => gu.GroupId == expenseDto.GroupId && gu.PersonId == personId);

            if (!isPersonInGroup)
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

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
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
