using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using debt_collector_api.Data;
using debt_collector_api.Models;
using debt_collector_api.Helpers;
using debt_collector_api.Responses;
using debt_collector_api.Requests;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;

        public PeopleController(
            DebtCollectorContext context, 
            AuthorizationHelper authorizationHelper
            )
        {
            _context = context;
            _authorizationHelper = authorizationHelper;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPerson()
        {
            return await _context.Persons.ToListAsync();
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDTO>> GetPerson(int id)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null || personId != id) return Unauthorized(new { message = "Invalid token" });

            var person = await _context.Persons
                .Include(p => p.Image)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            PersonDTO personDTO = new()
            {
                Id = person.Id,
                Username = person.Username,
                Image = person.Image,
                PrimaryCurrency = person.PrimaryCurrency
            };

            return personDTO;
        }

        // POST: api/People
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Email)) return BadRequest(new { message = "Email is required." } );

            var existingPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Email.ToLower() == person.Email.ToLower());

            if (existingPerson != null) return BadRequest(new { message = "Email is already taken." });

            if (string.IsNullOrWhiteSpace(person.Password)) return BadRequest(new { message = "Password is required." });

            person.Password = BCrypt.Net.BCrypt.HashPassword(person.Password);

            var dummyAvatar = await _context.Images.FirstOrDefaultAsync(i => i.Id == 1);
            if (dummyAvatar != null) person.Image = dummyAvatar;

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        [HttpPut("{id}/set-username")]
        public async Task<IActionResult> SetUsername(int id, [FromBody] SetUsernameRequest request)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null || personId != id) return Unauthorized(new { message = "Invalid token" });

            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();

            if (string.IsNullOrWhiteSpace(request.Username)) return BadRequest(new { message = "Username cannot be empty." });

            person.Username = request.Username;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/set-currency")]
        public async Task<IActionResult> SetCurrency(int id, [FromBody] SetCurrencyRequest request)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null || personId != id) return Unauthorized(new { message = "Invalid token" });

            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound();

            if (string.IsNullOrWhiteSpace(request.Currency)) return BadRequest(new { message = "Currency cannot be empty." });

            person.PrimaryCurrency = request.Currency;
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
