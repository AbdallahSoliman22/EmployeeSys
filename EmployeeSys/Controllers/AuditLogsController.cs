using EmployeeSys.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeSys.Controllers
{
	[Authorize(Policy = "AdminOnly")]
	[ApiController]
	[Route("api/[controller]")]
	public class AuditLogsController : ControllerBase
	{
		private readonly AuditLogService _logService;

		public AuditLogsController(AuditLogService logService)
		{
			_logService = logService;
		}

		[HttpGet]
		public async Task<IActionResult> GetLogs()
		{
			var logs = await _logService.GetAllAsync();
			return Ok(logs);
		}
	}

}
