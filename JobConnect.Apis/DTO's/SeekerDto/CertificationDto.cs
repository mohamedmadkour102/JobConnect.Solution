namespace JobConnect.Apis.DTO_s.SeekerDto
{
    public class CertificationDto
    {
        public string CertificationName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
