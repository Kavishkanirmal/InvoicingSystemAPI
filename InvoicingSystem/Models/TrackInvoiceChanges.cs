namespace InvoicingSystem.Models
{
    public class TrackInvoiceChanges
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public String? Operation { get; set; }
        public int InvoiceNumber { get; set; }

    }
}
