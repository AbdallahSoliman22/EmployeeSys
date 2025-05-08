using EmployeeSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSys.DAL.UnitOfWork
{
	public interface IUnitOfWork
	{
		IEmployeeRepository Employees { get; }
		Task<int> SaveChangesAsync();
	}
}
