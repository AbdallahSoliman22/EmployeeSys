using EmployeeSys.DAL.Repositories.Interfaces;
using EmployeeSys.Dtos;
using EmployeeSys.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.DAL.Repositories
{
	public class EmployeeRepository : IEmployeeRepository
	{
		private readonly ApplicationDbContext _context;

		public EmployeeRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<bool> EmployeeCodeExistsAsync(string code)
		{
			return await _context.Employees.AnyAsync(e => e.EmployeeCode == code);
		}

		public async Task AddEmployeeAsync(Employee employee)
		{
			_context.Employees.Add(employee);
			await _context.SaveChangesAsync();
		}
		public async Task<Employee?> GetByIdAsync(int id)
		{
			return await _context.Employees.FindAsync(id);
		}
		public async Task<IEnumerable<Employee>> GetFilteredAsync(EmployeeFilterDto filter)
		{
			var query = _context.Employees.AsQueryable();

			if (!string.IsNullOrEmpty(filter.Name))
				query = query.Where(e => e.Name.Contains(filter.Name));

			if (!string.IsNullOrEmpty(filter.JobTitle))
				query = query.Where(e => e.JobTitle.Contains(filter.JobTitle));

			if (filter.MinSalary.HasValue)
				query = query.Where(e => e.Salary >= filter.MinSalary.Value);

			if (filter.MaxSalary.HasValue)
				query = query.Where(e => e.Salary <= filter.MaxSalary.Value);

			return await query.ToListAsync();
		}
		public void Delete(Employee employee)
		{
			_context.Employees.Remove(employee);
		}
	}
}
