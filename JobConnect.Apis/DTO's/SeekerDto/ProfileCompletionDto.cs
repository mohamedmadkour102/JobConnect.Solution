using System;
using System.Collections.Generic;

namespace JobConnect.Apis.DTO_s.SeekerDto
{
    public class ProfileCompletionDto
    {
        public int TotalFields { get; set; }
        public int CompletedFields { get; set; }
        public double CompletionPercentage { get; set; }
        public List<FieldStatus> FieldDetails { get; set; }
    }

    public class FieldStatus
    {
        public string FieldName { get; set; }
        public string Status { get; set; }
        public string Value { get; set; }
    }
}