namespace BonaForMe.DomainCore.DTO
{
    public class DataTableDto
    {
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        public string SearchValue { get; set; }
        public int Skip { get; set; }
        public int PageSize { get; set; }
        public string Draw { get; set; }
    }
}
