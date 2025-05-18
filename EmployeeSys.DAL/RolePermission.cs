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
		public string RoleId { get; set; }
		public IdentityRole Role { get; set; } 

		public int PermissionId { get; set; }
		public Permission Permission { get; set; } 
	}

	public class UserPermission
	{
		public string UserId { get; set; } 
		public IdentityUser User { get; set; }

		public int PermissionId { get; set; }
		public Permission Permission { get; set; } 
	}
}
