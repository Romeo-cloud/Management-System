namespace Management_System_Api.Models.DTO
{
    // Used when creating a new credit sale
    public class CreateCreditSaleDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public decimal AmountOwed { get; set; }
    }

    // Used when recording a payment against a credit sale
    public class RecordPaymentDto
    {
        public Guid CreditSaleId { get; set; }
        public decimal Amount { get; set; }
    }

    // Used to send back credit sale data with balance & history
    public class CreditSaleResponseDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }

        public decimal AmountOwed { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<CreditPaymentResponseDto> Payments { get; set; } = new();
    }

    // Used to show payment details
    public class CreditPaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
