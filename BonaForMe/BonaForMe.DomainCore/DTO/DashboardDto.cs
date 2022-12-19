namespace BonaForMe.DomainCore.DTO
{
    public class DashboardDto
    {
        public int NewOrdersCount { get; set; }
        public int OrdersCountOfThisMount { get; set; }
        public int UserCount { get; set; }
        public int TotalOrderCount { get; set; }
    }
}