namespace InvoicingSystem.Models
{
    public class Customer
    {

        /* Properties */
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? ContactNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }

    }
}
