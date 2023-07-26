namespace InvoicingSystem.Models
{
    public class Invoice
    {

        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set;}
        public string? CustomerName { get; set;}
        public string? ProductName { get; set;}
        public int UnitsPerProduct { get; set;}
        public float UnitPricePerProduct { get; set;}
        public float TotalPricePerProduct { get; set;}
        public float DiscountPerProduct { get; set;}
        public int CustomerId { get; set;}
    }
}
