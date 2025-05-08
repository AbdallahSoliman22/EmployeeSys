using EmployeeSys.DAL;
using EmployeeSys.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.BLL.Services
{
	public class AuthService : IAuthService
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AuthService(
			IConfiguration configuration,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto)
		{
			var user = await _userManager.FindByNameAsync(loginDto.Username);
			if (user == null)
				return null;

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
			if (!result.Succeeded)
				return null;

			var roles = await _userManager.GetRolesAsync(user);

			var claims = new List<Claim>
			{

				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Email, user.Email)
			};

			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: expires,
				signingCredentials: creds);

			return new AuthResponseDto
			{
				Token = new JwtSecurityTokenHandler().WriteToken(token),
				Expiration = expires
			};
		}
		public async Task<bool> AddRoleToUserAsync(string email, string role)
		{
			if (!RolePermissions.AllowedRoles.Contains(role)) return false;

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return false;

			if (await _userManager.IsInRoleAsync(user, role)) return false;

			var result = await _userManager.AddToRoleAsync(user, role);
			return result.Succeeded;
		}

		public async Task<bool> RemoveRoleFromUserAsync(string email, string role)
		{
			if (!RolePermissions.AllowedRoles.Contains(role)) return false;

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return false;

			if (!await _userManager.IsInRoleAsync(user, role)) return false;

			var result = await _userManager.RemoveFromRoleAsync(user, role);
			return result.Succeeded;
		}
	}
}
