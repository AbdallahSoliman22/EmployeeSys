using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models.Dtos
{
	public class EmployeeReadDto
	{
		public int Id { get; set; }
		public string? FullName { get; set; } 
		public string? Code { get; set; }
		public decimal Salary { get; set; }
		public string? Department { get; set; } 
		public bool IsActive { get; set; }
	}
}
