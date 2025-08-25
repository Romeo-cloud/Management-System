namespace Management_System_Api.Models.DTOs
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Include sales linked to this invoice
        public List<SaleResponseDto> Sales { get; set; } = new();

    }
}
