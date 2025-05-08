using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models
{
	public class AuditLog
	{
		public int Id { get; set; }
		public string? UserId { get; set; }
		public string ActionType { get; set; } = "";  
		public string EntityName { get; set; } = "";  
		public string? EntityId { get; set; }
		public string? Changes { get; set; }         
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
		public IdentityUser? User { get; set; }
	}
}
