using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using debt_collector_api.Data;
using debt_collector_api.Models;
using System.Security.Claims;
using debt_collector_api.Responses;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly DebtCollectorContext _context;

        public PeopleController(DebtCollectorContext context)
        {
            _context = context;
        }

        // GET: api/People
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPerson()
        {
            return await _context.Persons.ToListAsync();
        }

        // GET: api/People/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        [HttpGet("{groupId}/people")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPeopleInGroup(int groupId)
        {
            var personId = GetCurrentPersonId();

            if (personId == 0 || personId == null) return Unauthorized();

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == groupId);

            if (!isPersonInGroup)
            {
                return Forbid();
            }

            var people = await _context.PersonGroups
                .Where(pg => pg.GroupId == groupId)
                .Include(pg => pg.Person)
                .Select(pg => new PersonDTO
                {
                    Id = pg.Person.Id,
                    Username = pg.Person.Username,
                })
                .ToListAsync();

            return Ok(people);
        }

        // POST: api/People
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            var existingPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Username.ToLower() == person.Username.ToLower());

            if (existingPerson != null)
            {
                return BadRequest(new { message = "Username is already taken." });
            }

            person.Password = BCrypt.Net.BCrypt.HashPassword(person.Password);
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
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
