namespace InvoicingSystem.Models
{
    public class TrackProductChanges
    {

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public String? Operation { get; set; }
        public int ProductId { get; set; }

    }
}
