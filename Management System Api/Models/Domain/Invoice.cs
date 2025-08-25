namespace Management_System_Api.Models.Domain
{
    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = null!;
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;
        public byte[] PdfBytes { get; set; } = Array.Empty<byte>();
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
