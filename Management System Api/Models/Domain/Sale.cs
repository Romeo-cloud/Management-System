namespace Management_System_Api.Models.Domain
{
    public class Sale
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = null!; // FK to Product.ProductId
        public Product Product { get; set; } = null!;
        public int QuantitySold { get; set; }
        public decimal UnitPrice { get; set; } // snapshot at sale time
        public decimal TotalPrice { get; set; }
        public string SoldByUserId { get; set; } = null!;
        public ApplicationUser SoldBy { get; set; } = null!;
        public string? CustomerName { get; set; }
        public string? CustomerContact { get; set; }
        public DateTime SoldAtUtc { get; set; } = DateTime.UtcNow;
        public Invoice? Invoice { get; set; }

    }
}
