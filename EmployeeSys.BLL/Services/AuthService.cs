using EmployeeSys.DAL;
using EmployeeSys.Models;
using EmployeeSys.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeSys.BLL.Services
{
	public class AuthService : IAuthService
	{
		private readonly ApplicationDbContext _context;
		private readonly JwtOptions _jwtOptions;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AuthService(
			ApplicationDbContext context,
			IOptions<JwtOptions> jwtOptions,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_jwtOptions = jwtOptions.Value;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto)
		{
			var user = await _userManager.FindByNameAsync(loginDto.Username)
				?? await _userManager.FindByEmailAsync(loginDto.Username);
			if (user == null)
				return null;

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
			if (!result.Succeeded)
				return null;

			return await IssueTokensAsync(user);
		}

		public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
				return null;

			var tokenHash = HashToken(refreshToken);
			var storedToken = await _context.RefreshTokens
				.Include(rt => rt.User)
				.SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);

			if (storedToken == null || storedToken.RevokedAtUtc.HasValue || storedToken.ExpiresAtUtc <= DateTime.UtcNow)
				return null;

			return await IssueTokensAsync(storedToken.User, storedToken);
		}

		public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
		{
			if (string.IsNullOrWhiteSpace(refreshToken))
				return false;

			var tokenHash = HashToken(refreshToken);
			var storedToken = await _context.RefreshTokens
				.SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);

			if (storedToken == null || storedToken.RevokedAtUtc.HasValue || storedToken.ExpiresAtUtc <= DateTime.UtcNow)
				return false;

			storedToken.RevokedAtUtc = DateTime.UtcNow;
			await _context.SaveChangesAsync();

			return true;
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

		private async Task<AuthResponseDto> IssueTokensAsync(IdentityUser user, RefreshToken? refreshTokenToRotate = null)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var now = DateTime.UtcNow;
			var accessTokenExpiresAt = now.AddMinutes(_jwtOptions.DurationInMinutes);
			var plainRefreshToken = GenerateRefreshToken();
			var refreshTokenHash = HashToken(plainRefreshToken);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
			};

			if (!string.IsNullOrWhiteSpace(user.Email))
			{
				claims.Add(new Claim(ClaimTypes.Email, user.Email));
				claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
			}

			claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
			var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

			var accessToken = new JwtSecurityToken(
				issuer: _jwtOptions.Issuer,
				audience: _jwtOptions.Audience,
				claims: claims,
				notBefore: now,
				expires: accessTokenExpiresAt,
				signingCredentials: credentials);

			if (refreshTokenToRotate != null)
			{
				refreshTokenToRotate.RevokedAtUtc = now;
				refreshTokenToRotate.ReplacedByTokenHash = refreshTokenHash;
			}

			_context.RefreshTokens.Add(new RefreshToken
			{
				UserId = user.Id,
				TokenHash = refreshTokenHash,
				CreatedAtUtc = now,
				ExpiresAtUtc = now.AddDays(_jwtOptions.RefreshTokenDurationInDays)
			});

			await _context.SaveChangesAsync();

			return new AuthResponseDto
			{
				Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
				RefreshToken = plainRefreshToken,
				Expiration = accessTokenExpiresAt,
				Username = user.UserName ?? string.Empty,
				Email = user.Email,
				Roles = roles.ToArray()
			};
		}

		private static string GenerateRefreshToken()
		{
			return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		}

		private static string HashToken(string token)
		{
			var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
			return Convert.ToHexString(hash);
		}
	}
}
