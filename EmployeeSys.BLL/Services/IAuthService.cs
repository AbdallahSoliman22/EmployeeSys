using EmployeeSys.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.BLL.Services
{
	public interface IAuthService
	{
		Task<AuthResponseDto?> AuthenticateAsync(LoginDto loginDto);
		Task<bool> AddRoleToUserAsync(string email, string role);
		Task<bool> RemoveRoleFromUserAsync(string email, string role);
	}
}
