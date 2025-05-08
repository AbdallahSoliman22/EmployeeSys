using EmployeeSys.DAL;
using EmployeeSys.Models.Dtos;
using EmployeeSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSys.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PermissionController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public PermissionController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost("assign-role-permissions")]
		public async Task<IActionResult> AssignRolePermissions([FromBody] UpdateRolePermissionsDto dto)
		{
			var role = await _roleManager.FindByNameAsync(dto.RoleName);
			if (role == null) return NotFound("Role not found");

			var existing = _context.RolePermissions.Where(rp => rp.RoleId == role.Id);
			_context.RolePermissions.RemoveRange(existing);

			var perms = _context.Permissions.Where(p => dto.Permissions.Contains(p.Name)).ToList();
			var rolePerms = perms.Select(p => new RolePermission { RoleId = role.Id, PermissionId = p.Id }).ToList();

			await _context.RolePermissions.AddRangeAsync(rolePerms);
			await _context.SaveChangesAsync();

			return Ok("Permissions updated for role");
		}

		[Authorize(Policy = "AdminOnly")]
		[HttpPost("assign-user-permissions")]
		public async Task<IActionResult> AssignUserPermissions([FromBody] UpdateUserPermissionsDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.UserEmail);
			if (user == null) return NotFound("User not found");

			var existing = _context.UserPermissions.Where(up => up.UserId == user.Id);
			_context.UserPermissions.RemoveRange(existing);

			var perms = _context.Permissions.Where(p => dto.Permissions.Contains(p.Name)).ToList();
			var userPerms = perms.Select(p => new UserPermission { UserId = user.Id, PermissionId = p.Id }).ToList();

			await _context.UserPermissions.AddRangeAsync(userPerms);
			await _context.SaveChangesAsync();

			return Ok("Permissions updated for user");
		}

		[Authorize]
		[HttpGet("my-permissions")]
		public async Task<IActionResult> GetCurrentUserPermissions()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null) return Unauthorized();

			var directPerms = _context.UserPermissions
				.Where(up => up.UserId == userId)
				.Select(up => up.Permission.Name);

			var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
			var rolePerms = _context.RolePermissions
				.Where(rp => roles.Contains(rp.Role.Name))
				.Select(rp => rp.Permission.Name);

			var allPerms = await directPerms.Union(rolePerms).Distinct().ToListAsync();
			return Ok(allPerms);
		}
	}

}
