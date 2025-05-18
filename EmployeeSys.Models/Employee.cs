using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.Models
{
		public class Employee
		{
			public int Id { get; set; }

			[Required]
			public required string EmployeeCode { get; set; }

			[Required]
			public string Name { get; set; }

			public string? JobTitle { get; set; }

			public decimal Salary { get; set; }

			public bool IsActive { get; set; } = true;

		}
}
