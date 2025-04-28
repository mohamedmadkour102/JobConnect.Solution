namespace JobConnect.Apis.Models
{
	public class JobTag
	{
		public int Id { get; set; }
		public int JobId { get; set; }
		public Job Job { get; set; }
		public string Tag { get; set; } = string.Empty;
	}
}
