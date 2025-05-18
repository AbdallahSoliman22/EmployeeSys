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
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}

	public class AuthResponseDto
	{
		public string Token { get; set; }
		public DateTime Expiration { get; set; }
	}

}
