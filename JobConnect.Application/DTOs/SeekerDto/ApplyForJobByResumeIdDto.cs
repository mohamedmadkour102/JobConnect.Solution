namespace JobConnect.Application.DTOs.SeekerDto
{
    public class ApplyForJobByResumeIdDto
    {
        public int JobId { get; set; }
        public int ResumeId { get; set; }
		public string CoverLetter { get; set; } 

    }
}