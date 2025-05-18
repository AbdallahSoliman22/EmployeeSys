using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EmployeeSys.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EmployeeSys.DAL
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<Employee> Employees { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<UserPermission> UserPermissions { get; set; }
		public DbSet<AuditLog> AuditLogs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Employee>(entity =>
			{
				entity.HasIndex(e => e.EmployeeCode)
					  .IsUnique();

				entity.Property(e => e.Salary)
					  .HasColumnType("decimal(18,2)");
			});
			modelBuilder.Entity<RolePermission>()
			.HasKey(rp => new { rp.RoleId, rp.PermissionId });

			modelBuilder.Entity<UserPermission>()
				.HasKey(up => new { up.UserId, up.PermissionId });
		}
	}
}
