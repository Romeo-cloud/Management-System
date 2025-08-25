namespace Management_System_Api.Models.Domain
{
    public class Product
    {
        // 🔑 Client-supplied alphanumeric PK
        public string ProductId { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        // ✅ Relationship with Category
        public int CategoryId { get; set; }  // FK
        public Category Category { get; set; } = null!;  // Navigation property

        // Existing relationship with Sales
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
