using MediatR;
using DebtCollector.Application.Auth.DTOs;
using DebtCollector.Application.Auth.Commands.Login;
using DebtCollector.Application.Auth.Commands.Logout;
using DebtCollector.Application.Auth.Commands.Refresh;
using DebtCollector.Application.Auth.Commands.SendVerificationEmail;
using DebtCollector.Application.Auth.Commands.VerifyEmail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DebtCollector.Infrastructure.Persistence;
using DebtCollector.Infrastructure.Services;
using DebtCollector.Api.Helpers;
using DebtCollector.Domain.Entities;
using DebtCollector.Application.Common.Interfaces;

namespace DebtCollector.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginCommand command)
        {
            try
            {
                var result = await _sender.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _sender.Send(new LogoutCommand());
                return Ok(new { message = "Logged out successfully" });
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh([FromBody] RefreshCommand command)
        {
            try
            {
                var result = await _sender.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("send-verification-email")]
        [Authorize]
        public async Task<IActionResult> SendVerificationEmail()
        {
            try
            {
                await _sender.Send(new SendVerificationEmailCommand());
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("verify-email")]
        [Authorize]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            try
            {
                await _sender.Send(command);
                return Ok(new { message = "Email verified successfully." });
            }
             catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                 return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                 return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


    }
}
