using EmployeeSys.Dtos;
using EmployeeSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EmployeeSys.DAL.Repositories.Interfaces
{
	public interface IEmployeeRepository
	{
		Task<bool> EmployeeCodeExistsAsync(string code);
		Task AddEmployeeAsync(Employee employee);
		Task<Employee?> GetByIdAsync(int id);
		Task<IEnumerable<Employee>> GetFilteredAsync(EmployeeFilterDto filter);
		void Delete(Employee employee);

	}
}
