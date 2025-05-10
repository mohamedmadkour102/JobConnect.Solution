namespace JobConnect.Apis.DTO_s.SeekerDto
{
	public class SavedJobDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public string JobType { get; set; } = string.Empty;
		public string PostedDate { get; set; } = string.Empty;
		public int ApplicationsCount { get; set; }
	}
}
