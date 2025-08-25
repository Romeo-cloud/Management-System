using System.Collections.Generic;

namespace Management_System_Api.Models.Domain
{
    public class Category
    {
        public int Id { get; set; } // PK
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
