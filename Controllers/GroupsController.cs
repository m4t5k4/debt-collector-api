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
using debt_collector_api.Responses;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly DebtCollectorContext _context;

        public GroupsController(DebtCollectorContext context)
        {
            _context = context;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroup()
        {
            return await _context.Groups.ToListAsync();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDTO>> GetGroup(int id)
        {
            var personId = GetCurrentPersonId();

            if (personId == 0 || personId == null) return Unauthorized();

            var userIsInGroup = await _context.PersonGroups.FirstOrDefaultAsync(pg => pg.GroupId == id && pg.PersonId == personId);

            if (userIsInGroup == null) return NotFound();

            var @group = await _context.Groups
                .Where(g => g.Id == id)
                .Include(g => g.Expenses)
                .Include(g => g.PersonGroups)
                .ThenInclude(pg => pg.Person)
                .Select(g => new GroupDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Password = g.Password,
                    Expenses = g.Expenses.Select(e => new ExpenseDTO
                    {
                        Id = e.Id,
                        GroupId = e.GroupId,
                        Name = e.Name,
                        Currency = e.Currency,
                        Orders = e.Orders.Select(o => new OrderDTO
                        {
                            Id = o.Id,
                            ExpenseId = o.ExpenseId,
                            Name = o.Name,
                            TotalCost = o.TotalCost,
                            Debtors = o.Debtors.Select(d => new DebtorDTO
                            {
                                Id = d.Id,
                                PersonId = d.PersonId,
                                OrderId = d.OrderId,
                                Value = d.Value,
                                HasPaid = d.HasPaid
                            }).ToList(),
                            Payers = o.Payers.Select(p => new PayerDTO
                            {
                                Id = p.Id,
                                PersonId = p.PersonId,
                                OrderId = p.OrderId,
                                Value = p.Value,
                            }).ToList()
                        }).ToList(),
                    }).ToList(),
                    People = g.PersonGroups.Select(pg => new PersonDTO
                    {
                        Id = pg.Person.Id,
                        Username = pg.Person.Username
                    }).ToList()
                }).FirstOrDefaultAsync();
                

            if (@group == null)
            {
                return NotFound();
            }

            return @group;
        }

        [HttpGet("my-groups")]
        public async Task<ActionResult<List<Group>>> GetGroupsForCurrentUser()
        {
            var personId = GetCurrentPersonId();

            if (personId == 0 || personId == null) return Unauthorized();

            var groups = await _context.PersonGroups
                .Where(pg => pg.PersonId == personId)
                .Select(pg => pg.Group)
                .ToListAsync();

            if (groups == null) return NotFound(new { message = "You are not a member of any group." });

            return Ok(groups);
        }

        [HttpGet("{groupId}/members")]
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

        [HttpPost("join-group")]
        public async Task<ActionResult> JoinGroup([FromBody] JoinGroupRequest request)
        {
            var personId = GetCurrentPersonId();

            if (personId == 0 || personId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var group = await _context.Groups
                .Where(g => g.Password.ToLower() == request.Password.ToLower())
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return NotFound(new { message = "Group with this password not found" });
            }

            var existingMember = await _context.PersonGroups
                .Where(pg => pg.PersonId == personId && pg.GroupId == group.Id)
                .FirstOrDefaultAsync();

            if (existingMember != null)
            {
                return BadRequest(new { message = "You are already a member of this group" });
            }

            var personGroup = new PersonGroup
            {
                PersonId = personId.Value,
                GroupId = group.Id
            };

            _context.PersonGroups.Add(personGroup);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully joined the group" });
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group @group)
        {
            var personId = GetCurrentPersonId();

            if (personId == 0 || personId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            @group.CreatedByPersonId = personId.Value;
            @group.ModifiedByPersonId = personId.Value;
            @group.CreatedOn = DateTime.UtcNow;
            @group.ModifiedOn = DateTime.UtcNow;
            @group.Password = await GenerateUniqueRandomPasswordAsync(5);

            _context.Groups.Add(@group);
            await _context.SaveChangesAsync();

            var personGroup = new PersonGroup
            {
                PersonId = personId.Value,
                GroupId = @group.Id
            };

            _context.PersonGroups.Add(personGroup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = @group.Id }, @group);
        }

        private async Task<string> GenerateUniqueRandomPasswordAsync(int length)
        {
            string password;
            var isUnique = false;
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            do
            {
                password = new string(Enumerable.Range(0, length)
                                               .Select(_ => chars[random.Next(chars.Length)])
                                               .ToArray());

                isUnique = !await _context.Groups.AnyAsync(g => g.Password == password);
            }
            while (!isUnique);

            return password;
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
