namespace JobConnect.Apis.DTO_s.EmployerDto
{
	public class JobDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public string JobType { get; set; } = string.Empty;
		public int DaysRemaining { get; set; }
		public int ApplicationsCount { get; set; }  
		public string PostedDate { get; set; } = string.Empty; 
	}
}
