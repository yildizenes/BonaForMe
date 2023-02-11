using System;

namespace BonaForMe.DomainCore.DTO
{
    public class ReportDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public decimal SalesValue { get; set; }
        public decimal RevenueValue { get; set; }
    }
}