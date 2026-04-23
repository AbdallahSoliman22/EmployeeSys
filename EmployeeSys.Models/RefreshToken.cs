using Microsoft.AspNetCore.Identity;

namespace EmployeeSys.Models
{
	public class RefreshToken
	{
		public int Id { get; set; }
		public string UserId { get; set; } = string.Empty;
		public IdentityUser User { get; set; } = default!;
		public string TokenHash { get; set; } = string.Empty;
		public DateTime CreatedAtUtc { get; set; }
		public DateTime ExpiresAtUtc { get; set; }
		public DateTime? RevokedAtUtc { get; set; }
		public string? ReplacedByTokenHash { get; set; }
	}
}
