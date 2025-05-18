using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models.Dtos
{
	public class CreateEmployeeDto
	{
		[Required]
		public string FullName { get; set; }
		[Required]
		public string Code { get; set; }
		public decimal Salary { get; set; }
		public string Department { get; set; } 
		public bool IsActive { get; set; } = true;
	}

}
