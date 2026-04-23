using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models.Dtos
{
	public class LoginDto
	{
		[Required]
		public string Username { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
	}

	public class AuthResponseDto
	{
		public string Token { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime Expiration { get; set; }
		public string TokenType { get; set; } = "Bearer";
		public string Username { get; set; } = string.Empty;
		public string? Email { get; set; }
		public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();
	}

	public class RefreshTokenRequestDto
	{
		[Required]
		public string RefreshToken { get; set; } = string.Empty;
	}

	public class RevokeRefreshTokenRequestDto
	{
		[Required]
		public string RefreshToken { get; set; } = string.Empty;
	}

}
