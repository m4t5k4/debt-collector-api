using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Infrastructure.Persistence;
using DebtCollector.Domain.Entities;
using System.Security.Claims;
using DebtCollector.Application.DTOs;
using Microsoft.AspNetCore.SignalR;
using DebtCollector.Application.Groups.Queries.GetGroups;
using DebtCollector.Application.Groups.Queries.GetGroupById;
using DebtCollector.Application.Groups.Queries.GetMyGroups;
using DebtCollector.Application.Groups.Queries.GetGroupMembers;
using DebtCollector.Application.Groups.Commands.CreateGroup;
using DebtCollector.Application.Groups.Commands.JoinGroup;
using DebtCollector.Application.Groups.Commands.DeleteGroup;
using DebtCollector.Application.Groups.Commands.RemoveGroupMember;
using MediatR;
using DebtCollector.Api.Hubs;

namespace DebtCollector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly ISender _sender;

        public GroupsController(ISender sender)
        {
            _sender = sender;
        }

        // GET: api/Groups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDTO>>> GetGroup()
        {
            return Ok(await _sender.Send(new GetGroupsQuery()));
        }

        // GET: api/Groups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDTO>> GetGroup(int id)
        {
            try
            {
                return Ok(await _sender.Send(new GetGroupByIdQuery { Id = id }));
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (System.Collections.Generic.KeyNotFoundException) { return NotFound(); }
        }

        [HttpGet("my-groups")]
        public async Task<ActionResult<List<GroupDTO>>> GetGroupsForCurrentUser()
        {
            try
            {
                return Ok(await _sender.Send(new GetMyGroupsQuery()));
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpGet("{groupId}/members")]
        public async Task<ActionResult<IEnumerable<PersonDTO>>> GetPeopleInGroup(int groupId)
        {
            try
            {
                return Ok(await _sender.Send(new GetGroupMembersQuery { GroupId = groupId }));
            }
            catch (UnauthorizedAccessException) { return Forbid(); } // Mapping Unauthorized to Forbid as per original logic somewhat? Or Unauthorized? Original used Forbid() for logic check, Unauthorized for missing token.
        }

        [HttpPost("join-group")]
        public async Task<ActionResult> JoinGroup([FromBody] JoinGroupCommand command)
        {
            try
            {
                var groupId = await _sender.Send(command);

                var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<GroupHub>>();
                await hubContext.Clients.Group(groupId).SendAsync("GroupUpdated");

                return Ok(new { message = "Successfully joined the group" });
            }
            catch (UnauthorizedAccessException) { return Unauthorized(new { message = "User not authenticated" }); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // POST: api/Groups
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(CreateGroupCommand command)
        {
            try
            {
                var group = await _sender.Send(command);
                return CreatedAtAction("GetGroup", new { id = group.Id }, group);
            }
            catch (UnauthorizedAccessException) { return Unauthorized(new { message = "Invalid token" }); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                await _sender.Send(new DeleteGroupCommand { Id = id });
                return NoContent();
            }
             catch (UnauthorizedAccessException) { return Unauthorized(new { message = "Invalid token" }); }
             catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }

        [HttpDelete("{groupId}/members/{personId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int personId)
        {
            try
            {
                await _sender.Send(new RemoveGroupMemberCommand { GroupId = groupId, PersonId = personId });
                return Ok(new { message = "Member removed successfully" });
            }
            catch (UnauthorizedAccessException) { return Unauthorized(new { message = "Invalid token" }); }
            catch (System.Collections.Generic.KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        }
    }
}
