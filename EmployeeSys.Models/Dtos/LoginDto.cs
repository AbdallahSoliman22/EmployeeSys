using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models.Dtos
{
	public class LoginDto
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class AuthResponseDto
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
	}

}
