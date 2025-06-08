namespace JobConnect.Apis.DTO_s.SeekerDto
{
    public class CompanyWorkedAtDto
    {
        public string CompanyName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
