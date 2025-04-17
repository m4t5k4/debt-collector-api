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
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using Group = debt_collector_api.Models.Group;
using debt_collector_api.Helpers;

namespace debt_collector_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly DebtCollectorContext _context;
        private readonly AuthorizationHelper _authorizationHelper;

        public GroupsController(
            DebtCollectorContext context,
            AuthorizationHelper authorizationHelper
            )
        {
            _context = context;
            _authorizationHelper = authorizationHelper;
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
            var personId = _authorizationHelper.GetCurrentPersonId();    

            if (personId == null) return Unauthorized();

            var userIsInGroup = await _context.PersonGroups.FirstOrDefaultAsync(pg => pg.GroupId == id && pg.PersonId == personId);

            if (userIsInGroup == null) return NotFound();

            var @group = await _context.Groups
                .Where(g => g.Id == id)
                .Include(g => g.Image)
                .Include(g => g.Expenses)
                .Include(g => g.PersonGroups)
                .ThenInclude(pg => pg.Person)
                .ThenInclude(p => p.Image)
                .Select(g => new GroupDTO
                {
                    Id = g.Id,
                    Name = g.Name,
                    Password = g.Password,
                    Image = g.Image ?? null,
                    CreatedOn = g.CreatedOn,
                    CreatedByPersonId = g.CreatedByPersonId,
                    ModifiedOn = g.ModifiedOn,
                    ModifiedByPersonId = g.ModifiedByPersonId,
                    Expenses = g.Expenses.Select(e => new ExpenseDTO
                    {
                        Id = e.Id,
                        GroupId = e.GroupId,
                        Name = e.Name,
                        Currency = e.Currency,
                        CreatedOn = e.CreatedOn,
                        CreatedByPersonId = e.CreatedByPersonId,
                        ModifiedOn = e.ModifiedOn,
                        ModifiedByPersonId = e.ModifiedByPersonId,
                        Orders = e.Orders.Select(o => new OrderDTO
                        {
                            Id = o.Id,
                            ExpenseId = o.ExpenseId,
                            Name = o.Name,
                            TotalCost = o.TotalCost,
                            CreatedOn = o.CreatedOn,
                            CreatedByPersonId = o.CreatedByPersonId,
                            ModifiedOn = o.ModifiedOn,
                            ModifiedByPersonId = o.ModifiedByPersonId,
                            Debtors = o.Debtors.Select(d => new DebtorDTO
                            {
                                Id = d.Id,
                                PersonId = d.PersonId,
                                OrderId = d.OrderId,
                                Value = d.Value,
                                HasPaid = d.HasPaid,
                                CreatedOn = d.CreatedOn,
                                CreatedByPersonId = d.CreatedByPersonId,
                                ModifiedOn = d.ModifiedOn,
                                ModifiedByPersonId = d.ModifiedByPersonId,
                            }).ToList(),
                            Payers = o.Payers.Select(p => new PayerDTO
                            {
                                Id = p.Id,
                                PersonId = p.PersonId,
                                OrderId = p.OrderId,
                                Value = p.Value,
                                CreatedOn = p.CreatedOn,
                                CreatedByPersonId = p.CreatedByPersonId,
                                ModifiedOn = p.ModifiedOn,
                                ModifiedByPersonId = p.ModifiedByPersonId,
                            }).ToList()
                        }).ToList(),
                    }).ToList(),
                    People = g.PersonGroups.Select(pg => new PersonDTO
                    {
                        Id = pg.Person.Id,
                        Username = pg.Person.Username,
                        Image = pg.Person.Image,
                    }).ToList()
                }).FirstOrDefaultAsync();
                

            if (@group == null) return NotFound();

            return @group;
        }

        [HttpGet("my-groups")]
        public async Task<ActionResult<List<Group>>> GetGroupsForCurrentUser()
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized();

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
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized();

            var isPersonInGroup = await _context.PersonGroups
                .AnyAsync(pg => pg.PersonId == personId && pg.GroupId == groupId);

            if (!isPersonInGroup) return Forbid();

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
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "User not authenticated" });

            var group = await _context.Groups
                .Where(g => g.Password.ToLower() == request.Password.ToLower())
                .FirstOrDefaultAsync();

            if (group == null) return NotFound(new { message = "Group with this password not found" });

            var existingMember = await _context.PersonGroups
                .Where(pg => pg.PersonId == personId && pg.GroupId == group.Id)
                .FirstOrDefaultAsync();

            if (existingMember != null) return BadRequest(new { message = "You are already a member of this group" });

            var personGroup = new PersonGroup
            {
                PersonId = personId.Value,
                GroupId = group.Id
            };

            _context.PersonGroups.Add(personGroup);
            await _context.SaveChangesAsync();

            var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<GroupHub>>();
            await hubContext.Clients.Group(group.Id.ToString()).SendAsync("GroupUpdated");

            return Ok(new { message = "Successfully joined the group" });
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group @group)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            @group.CreatedByPersonId = personId.Value;
            @group.ModifiedByPersonId = personId.Value;
            @group.CreatedOn = DateTime.UtcNow;
            @group.ModifiedOn = DateTime.UtcNow;
            @group.Password = await _authorizationHelper.GenerateUniqueRandomPasswordAsync(5);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var personId = _authorizationHelper.GetCurrentPersonId();

            if (personId == null) return Unauthorized(new { message = "Invalid token" });

            var group = await _context.Groups
                .Include(g => g.PersonGroups)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null) return NotFound(new { message = "Group not found"});

            _context.PersonGroups.RemoveRange(group.PersonGroups);

            _context.Groups.Remove(group);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{groupId}/members/{personId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int personId)
        {
            var currentPersonId = _authorizationHelper.GetCurrentPersonId();

            if (currentPersonId == null) return Unauthorized(new { message = "Invalid token" });

            var group = await _context.Groups
                .Include(g => g.PersonGroups)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group == null) return NotFound(new { message = "Group not found" });

            var personGroup = group.PersonGroups.FirstOrDefault(pg => pg.PersonId == personId);
            if (personGroup == null)
                return NotFound(new { message = "Member not found in this group" });

            _context.PersonGroups.Remove(personGroup);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Member removed successfully" });
        }
    }
}
