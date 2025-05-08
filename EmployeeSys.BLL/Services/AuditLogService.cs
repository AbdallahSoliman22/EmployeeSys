using EmployeeSys.DAL;
using EmployeeSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmployeeSys.BLL.Services
{
	public class AuditLogService
	{
		private readonly ApplicationDbContext _context;

		public AuditLogService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task LogAsync(string userId, string actionType, string entityName, string? entityId, object? changes)
		{
			var log = new AuditLog
			{
				UserId = userId,
				ActionType = actionType,
				EntityName = entityName,
				EntityId = entityId,
				Changes = changes != null ? JsonSerializer.Serialize(changes) : null,
				Timestamp = DateTime.UtcNow
			};

			_context.AuditLogs.Add(log);
			await _context.SaveChangesAsync();
		}

		public async Task<List<AuditLog>> GetAllAsync()
		{
			return await _context.AuditLogs.OrderByDescending(l => l.Timestamp).ToListAsync();
		}
	}
}