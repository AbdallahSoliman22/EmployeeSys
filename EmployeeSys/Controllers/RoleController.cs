using EmployeeSys.BLL.Services;
using EmployeeSys.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSys.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RoleController : ControllerBase
	{
		private readonly IAuthService _authService;

		public RoleController(IAuthService authService)
		{
			_authService = authService;
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost("add-role")]
		public async Task<IActionResult> AddRole([FromBody] RoleUpdateDto dto)
		{
			var success = await _authService.AddRoleToUserAsync(dto.UserEmail, dto.Role);
			if (!success) return BadRequest("Failed to add role. Check role validity or user existence.");
			return Ok("Role added.");
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost("remove-role")]
		public async Task<IActionResult> RemoveRole([FromBody] RoleUpdateDto dto)
		{
			var success = await _authService.RemoveRoleFromUserAsync(dto.UserEmail, dto.Role);
			if (!success) return BadRequest("Failed to remove role. Check role validity or user existence.");
			return Ok("Role removed.");
		}
	}
}
