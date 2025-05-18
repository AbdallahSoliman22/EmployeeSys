using EmployeeSys.DAL.UnitOfWork;
using EmployeeSys.Dtos;
using EmployeeSys.Models;
using EmployeeSys.Models.Dtos;
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

		public async Task<(bool Success, string Message)> AddEmployeeAsync(CreateEmployeeDto dto)
		{
			if (await _unitOfWork.Employees.EmployeeCodeExistsAsync(dto.Code))
				return (false, "Employee code must be unique.");

			var employee = new Employee
			{
				Name = dto.FullName,
				JobTitle = dto.Department,
				Salary = dto.Salary,
				IsActive = dto.IsActive,
				EmployeeCode = dto.Code,
			};

			await _unitOfWork.Employees.AddEmployeeAsync(employee);
			await _unitOfWork.SaveChangesAsync();

			string userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";
			await _auditLogService.LogAsync(userId, "Create", "Employee", employee.Id.ToString(), employee);
			return (true, "Employee added successfully.");
		}

		public async Task<(bool Success, string Message)> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
		{
			var existing = await _unitOfWork.Employees.GetByIdAsync(id);
			if (existing == null)
				return (false, "Employee not found.");

			var changes = new
			{
				Old = new { existing.Name, existing.JobTitle,existing.Salary, existing.IsActive },
				New = new { dto.FullName, dto.Department,dto.Salary, dto.IsActive }
			};

			existing.Name = dto.FullName;
			existing.JobTitle = dto.Department;
			existing.Salary = dto.Salary;
			existing.IsActive = dto.IsActive;

			await _unitOfWork.SaveChangesAsync();

			await _auditLogService.LogAsync(GetCurrentUserId(), "Update", "Employee", id.ToString(), changes);

			return (true, "Employee updated successfully.");
		}

		public async Task<IEnumerable<EmployeeReadDto>> GetFilteredEmployeesAsync(EmployeeFilterDto filter)
		{
			var employees = await _unitOfWork.Employees.GetFilteredAsync(filter);

			return employees.Select(e => new EmployeeReadDto
			{
				Id = e.Id,
				FullName = e.Name,
				Code = e.EmployeeCode,
				Department = e.JobTitle,
				IsActive = e.IsActive
			});
		}

		public async Task<EmployeeReadDto?> GetEmployeeByIdAsync(int id)
		{
			var employee = await _unitOfWork.Employees.GetByIdAsync(id);
			if (employee == null) return null;

			return new EmployeeReadDto
			{
				Id = employee.Id,
				FullName = employee.Name,
				Code = employee.EmployeeCode,
				Department = employee.JobTitle,
				IsActive = employee.IsActive
			};
		}

		public async Task<bool> DeleteEmployeeAsync(int id)
		{
			var employee = await _unitOfWork.Employees.GetByIdAsync(id);
			if (employee == null) return false;

			if (employee.IsActive)
				throw new InvalidOperationException("Cannot delete an active employee.");

			_unitOfWork.Employees.Delete(employee);
			await _unitOfWork.SaveChangesAsync();

			await _auditLogService.LogAsync(GetCurrentUserId(), "Delete", "Employee", id.ToString(), null);

			return true;
		}
	}
}
