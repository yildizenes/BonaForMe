namespace BonaForMe.DomainCore.DTO.PDFModels
{
    public class ItemRow
    {
        public string Item { get; set; }
        public string Description { get; set; }
        public decimal PerUnit { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal ExtPrice { get; set; }
        public decimal Vat { get; set; }
        public bool IsCampaignProduct { get; set; }

        public static ItemRow Make(string item, string description, decimal perUnit, int qty, decimal price, decimal extPrice, decimal vat, bool isCampaignProduct)
        {
            return new ItemRow()
            {
                Item = item,
                Description = description,
                PerUnit = perUnit,
                Qty = qty,
                Price = price,
                ExtPrice = extPrice,
                Vat = vat,
                IsCampaignProduct = isCampaignProduct,
            };
        }
    }
}