namespace EmployeeSys.Dtos
{
	public class EmployeeFilterDto
	{
		public string? Name { get; set; }
		public string? JobTitle { get; set; }
		public decimal? MinSalary { get; set; }
		public decimal? MaxSalary { get; set; }
	}
}
