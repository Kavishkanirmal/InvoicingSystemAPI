namespace InvoicingSystem.Models
{
    public class TrackCustomerChanges
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public String? Operation { get; set; }
        public int CustomerId { get; set; }

    }
}
