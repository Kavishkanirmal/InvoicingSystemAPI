namespace InvoicingSystem.Models
{
    public class Product
    {

        /* Properties */
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public float PurchasePrice { get; set; }
        public float SellingPrice { get; set; }
        public int Quantity { get; set; }

    }
}
