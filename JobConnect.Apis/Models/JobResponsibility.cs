namespace JobConnect.Apis.Models
{
	public class JobResponsibility
	{
		public int Id { get; set; }
		public int JobId { get; set; }
		public Job Job { get; set; }
		public string Responsibility { get; set; } = string.Empty;
	}
}
