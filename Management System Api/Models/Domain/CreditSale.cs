namespace Management_System_Api.Models.Domain
{
    public class CreditSale
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Customer details
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }

        // Finance details
        public decimal AmountOwed { get; set; }   // Total credit amount
        public decimal AmountPaid { get; set; }   // Total payments made

        public decimal Balance => AmountOwed - AmountPaid; // Computed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for payment history
        public ICollection<CreditPayment> Payments { get; set; } = new List<CreditPayment>();
    }

    public class CreditPayment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CreditSaleId { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        public CreditSale CreditSale { get; set; }
    }
}
