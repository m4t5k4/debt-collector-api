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
            return await _context.Group.ToListAsync();
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var @group = await _context.Group.FindAsync(id);

            if (@group == null)
            {
                return NotFound();
            }

            return @group;
        }

        [HttpGet("my-groups")]
        public async Task<ActionResult<List<Group>>> GetGroupsForCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (userId == 0) return Unauthorized();

            var groups = await _context.PersonGroup
                .Where(pg => pg.PersonId == userId)
                .Select(pg => pg.Group)
                .ToListAsync();

            if (groups == null) return NotFound(new { message = "You are not a member of any group." });

            return Ok(groups);
        }

        [HttpPost("join-group")]
        public async Task<ActionResult> JoinGroup([FromBody] JoinGroupRequest request)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var group = await _context.Group
                .Where(g => g.Password.ToLower() == request.Password.ToLower())
                .FirstOrDefaultAsync();

            if (group == null)
            {
                return NotFound(new { message = "Group with this password not found" });
            }

            var existingMember = await _context.PersonGroup
                .Where(pg => pg.PersonId == userId && pg.GroupId == group.Id)
                .FirstOrDefaultAsync();

            if (existingMember != null)
            {
                return BadRequest(new { message = "You are already a member of this group" });
            }

            var personGroup = new PersonGroup
            {
                PersonId = userId.Value,
                GroupId = group.Id
            };

            _context.PersonGroup.Add(personGroup);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully joined the group" });
        }



        // PUT: api/Groups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group @group)
        {
            if (id != @group.Id)
            {
                return BadRequest();
            }

            _context.Entry(@group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        // POST: api/Groups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group @group)
        {
            var personId = GetCurrentUserId();

            if (personId == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            @group.CreatedByPersonId = personId.Value;
            @group.ModifiedByPersonId = personId.Value;
            @group.CreatedOn = DateTime.UtcNow;
            @group.ModifiedOn = DateTime.UtcNow;
            @group.Password = await GenerateUniqueRandomPasswordAsync(5);

            _context.Group.Add(@group);
            await _context.SaveChangesAsync();

            var personGroup = new PersonGroup
            {
                PersonId = personId.Value,
                GroupId = @group.Id
            };

            _context.PersonGroup.Add(personGroup);
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

                isUnique = !await _context.Group.AnyAsync(g => g.Password == password);
            }
            while (!isUnique);

            return password;
        }

        // DELETE: api/Groups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var @group = await _context.Group.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            _context.Group.Remove(@group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(int id)
        {
            return _context.Group.Any(e => e.Id == id);
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
