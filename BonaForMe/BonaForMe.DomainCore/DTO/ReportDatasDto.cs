using System.Collections.Generic;

namespace BonaForMe.DomainCore.DTO
{
    public class ReportDatasDto
    {
        public ReportColumnDto RevenueTaxSummary { get; set; }
        public List<ReportColumnDto> Taxes { get; set; }
        public List<ReportColumnDto> RevenueByCategories { get; set; }
        public List<ReportColumnDto> RevenueByEmployees { get; set; }
        public List<ReportColumnDto> PaymentMethods { get; set; }
    }
}