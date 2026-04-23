using EmployeeSys.BLL.Services;
using EmployeeSys.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSys.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}
		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			var result = await _authService.AuthenticateAsync(loginDto);
			if (result == null)
				return Unauthorized();

			return Ok(result);
		}

		[AllowAnonymous]
		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
		{
			var result = await _authService.RefreshTokenAsync(request.RefreshToken);
			if (result == null)
				return Unauthorized("Invalid or expired refresh token.");

			return Ok(result);
		}

		[AllowAnonymous]
		[HttpPost("revoke")]
		public async Task<IActionResult> Revoke([FromBody] RevokeRefreshTokenRequestDto request)
		{
			var revoked = await _authService.RevokeRefreshTokenAsync(request.RefreshToken);
			if (!revoked)
				return NotFound("Refresh token not found or already inactive.");

			return NoContent();
		}
	}

}
