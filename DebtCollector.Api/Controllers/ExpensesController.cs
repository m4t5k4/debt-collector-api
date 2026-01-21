using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Infrastructure.Persistence;
using DebtCollector.Domain.Entities;
using DebtCollector.Application.Expenses.Commands.CreateExpense;
using DebtCollector.Application.Expenses.Commands.UpdateExpense;
using DebtCollector.Application.Expenses.Commands.DeleteExpense;
using DebtCollector.Application.Expenses.Queries.GetExpenseById;
using MediatR;
using DebtCollector.Application.DTOs;

namespace DebtCollector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ISender _sender;

        public ExpensesController(ISender sender)
        {
            _sender = sender;
        }

        // GET: api/Expenses/5
        // GET: api/Expenses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            try
            {
                return Ok(await _sender.Send(new GetExpenseByIdQuery { Id = id }));
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(new { message = "Expense not found" }); }
        }

        // POST: api/Expenses
        // POST: api/Expenses
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense(CreateExpenseCommand command)
        {
            try
            {
                var expense = await _sender.Send(command);
                return CreatedAtAction("GetExpense", new { id = expense.Id }, expense);
            }
             catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
             catch (System.Collections.Generic.KeyNotFoundException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{expenseId}")]
        public async Task<IActionResult> UpdateExpense(int expenseId, [FromBody] UpdateExpenseCommand command)
        {
            if (expenseId != command.ExpenseId) return BadRequest("Expense ID mismatch");

            try
            {
                return Ok(await _sender.Send(command));
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            try
            {
                await _sender.Send(new DeleteExpenseCommand { Id = id });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
        }
    }
}
