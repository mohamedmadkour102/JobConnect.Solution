namespace JobConnect.Apis.DTO_s.Admin
{
	public class JobDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Status { get; set; }
		public int ApplicationsCount { get; set; }
		public string JobType { get; set; }
		public int DaysRemaining { get; set; }
		public string PostedDate { get; set; }
		public string Location { get; set; }
		public List<string> Tags { get; set; }
		public List<string> Responsibilities { get; set; }
		public string EmployerName { get; set; }
	}
}
