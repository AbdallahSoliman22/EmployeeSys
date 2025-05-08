using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models
{
	public class RolePermission
	{
		public string RoleId { get; set; } = null!;
		public IdentityRole Role { get; set; } = null!;

		public int PermissionId { get; set; }
		public Permission Permission { get; set; } = null!;
	}

	public class UserPermission
	{
		public string UserId { get; set; } = null!;
		public IdentityUser User { get; set; } = null!;

		public int PermissionId { get; set; }
		public Permission Permission { get; set; } = null!;
	}
}
