using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Infrastructure.Persistence;
using DebtCollector.Domain.Entities;
using DebtCollector.Application.People.Commands.RegisterPerson;
using DebtCollector.Application.People.Commands.SetUsername;
using DebtCollector.Application.People.Commands.SetCurrency;
using DebtCollector.Application.People.Queries.GetPerson;
using DebtCollector.Application.People.Queries.GetPeople;
using MediatR;
using DebtCollector.Application.DTOs;

namespace DebtCollector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ISender _sender;

        public PeopleController(ISender sender)
        {
            _sender = sender;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPerson()
        {
            return Ok(await _sender.Send(new GetPeopleQuery()));
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDTO>> GetPerson(int id)
        {
            try
            {
                return Ok(await _sender.Send(new GetPersonQuery { Id = id }));
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
        }

        // POST: api/People
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(RegisterPersonCommand command)
        {
            try
            {
                var person = await _sender.Send(command);
                return CreatedAtAction("GetPerson", new { id = person.Id }, person);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}/set-username")]
        public async Task<IActionResult> SetUsername(int id, [FromBody] SetUsernameCommand command)
        {
            try
            {
                command.PersonId = id; // Set from route parameter
                await _sender.Send(command);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

        [HttpPut("{id}/set-currency")]
        public async Task<IActionResult> SetCurrency(int id, [FromBody] SetCurrencyCommand command)
        {
            try
            {
                command.PersonId = id; // Set from route parameter
                await _sender.Send(command);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { message = ex.Message }); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        }

    }
}
