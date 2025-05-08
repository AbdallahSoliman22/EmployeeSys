using EmployeeSys.DAL.Repositories.Interfaces;
using EmployeeSys.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.DAL.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;

		public IEmployeeRepository Employees { get; }

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			Employees = new EmployeeRepository(_context);
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
