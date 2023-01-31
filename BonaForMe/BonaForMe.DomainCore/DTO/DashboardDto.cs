namespace BonaForMe.DomainCore.DTO
{
    public class DashboardDto
    {
        public int NewOrdersCount { get; set; }
        public int OrdersCountOfThisMonth { get; set; }
        public int PendingApprovalUserCount { get; set; }
        public int TotalOrderCount { get; set; }
    }
}