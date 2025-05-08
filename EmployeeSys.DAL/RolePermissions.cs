using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.DAL
{
	public static class RolePermissions
	{
		public static readonly List<string> AllowedRoles = new() { "Admin", "User" };
	}

}
