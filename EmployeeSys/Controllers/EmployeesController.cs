using EmployeeSys.BLL.Services;
using EmployeeSys.Dtos;
using EmployeeSys.Models;
using EmployeeSys.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSys.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	
	public class EmployeesController : ControllerBase
	{
		private readonly EmployeeService _service;

		public EmployeesController(EmployeeService service)
		{
			_service = service;
		}
		[Authorize(Policy = "AdminOnly")]
		[HttpPost("add")]
		public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _service.AddEmployeeAsync(dto);

			if (!result.Success)
				return Conflict(result.Message);

			return Ok(result.Message);
		}
		[Authorize(Policy = "AdminOnly")]
		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var (success, message) = await _service.UpdateEmployeeAsync(id, dto);
			if (!success)
				return Conflict(message);

			return Ok(message);
		}
		[Authorize(Roles = "User")]
		[HttpGet("filter")]
		public async Task<IActionResult> FilterEmployees([FromQuery] EmployeeFilterDto filter)
		{
			var employees = await _service.GetFilteredEmployeesAsync(filter);
			return Ok(employees);
		}
		[Authorize(Roles = "User")]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetEmployeeById(int id, [FromQuery] string? expand)
		{
			var employee = await _service.GetEmployeeByIdAsync(id);
			if (employee == null)
				return NotFound();

			var result = new
			{
				employee.Id,
				employee.Code,
				employee.FullName,
				employee.IsActive,
				Salary = expand == "yes" ? new
				{
					Amount = employee.Salary,
					Bonus = employee.Salary * (decimal)0.15
				} : null
			};

			return Ok(result);
		}
		[Authorize(Policy = "AdminOnly")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteEmployee(int id)
		{
			try
			{
				var result = await _service.DeleteEmployeeAsync(id);
				if (!result)
					return NotFound();

				return NoContent();
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

	}
}
