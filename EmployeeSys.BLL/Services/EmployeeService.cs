using EmployeeSys.DAL.UnitOfWork;
using EmployeeSys.Dtos;
using EmployeeSys.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EmployeeSys.BLL.Services
{

	public class EmployeeService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly AuditLogService _auditLogService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public EmployeeService(IUnitOfWork unitOfWork, AuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
		{
			_unitOfWork = unitOfWork;
			_auditLogService = auditLogService;
			_httpContextAccessor = httpContextAccessor;
		}
		private string GetCurrentUserId()
		{
			return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
			   ?? "System";
		}
		public async Task<(bool Success, string Message)> AddEmployeeAsync(Employee employee)
		{
			employee.UserId = GetCurrentUserId();
			if (await _unitOfWork.Employees.EmployeeCodeExistsAsync(employee.EmployeeCode))
				return (false, "Employee code must be unique.");

			await _unitOfWork.Employees.AddEmployeeAsync(employee);
			await _unitOfWork.SaveChangesAsync();

			await _auditLogService.LogAsync(GetCurrentUserId(), "Create", "Employee", employee.Id.ToString(), employee);

			return (true, "Employee added successfully.");
		}
		public async Task<(bool Success, string Message)> UpdateEmployeeAsync(int id, Employee updated)
		{
			var existing = await _unitOfWork.Employees.GetByIdAsync(id);
			if (existing == null)
				return (false, "Employee not found.");

			if (existing.EmployeeCode != updated.EmployeeCode)
				return (false, "Employee code cannot be changed.");

			var changes = new
			{
				Old = new { existing.Name, existing.JobTitle, existing.Salary, existing.IsActive },
				New = new { updated.Name, updated.JobTitle, updated.Salary, updated.IsActive }
			};

			existing.Name = updated.Name;
			existing.JobTitle = updated.JobTitle;
			existing.Salary = updated.Salary;
			existing.IsActive = updated.IsActive;

			await _unitOfWork.SaveChangesAsync();

			await _auditLogService.LogAsync(GetCurrentUserId(), "Update", "Employee", id.ToString(), changes);

			return (true, "Employee updated successfully.");
		}
		public async Task<IEnumerable<Employee>> GetFilteredEmployeesAsync(EmployeeFilterDto filter)
		{
			return await _unitOfWork.Employees.GetFilteredAsync(filter);
		}
		public async Task<Employee?> GetEmployeeByIdAsync(int id)
		{
			return await _unitOfWork.Employees.GetByIdAsync(id);
		}
		public async Task<bool> DeleteEmployeeAsync(int id)
		{
			var employee = await _unitOfWork.Employees.GetByIdAsync(id);
			if (employee == null)
				return false;

			if (employee.IsActive)
				throw new InvalidOperationException("Cannot delete an active employee.");

			_unitOfWork.Employees.Delete(employee);
			await _unitOfWork.SaveChangesAsync();

			await _auditLogService.LogAsync(GetCurrentUserId(), "Delete", "Employee", id.ToString(), null);

			return true;
		}

	}

}
