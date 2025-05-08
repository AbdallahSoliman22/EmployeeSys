using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models.Dtos
{
	public class UpdateRolePermissionsDto
	{
		public string RoleName { get; set; } = null!;
		public List<string> Permissions { get; set; } = new();
	}

	public class UpdateUserPermissionsDto
	{
		public string UserEmail { get; set; } = null!;
		public List<string> Permissions { get; set; } = new();
	}

}
