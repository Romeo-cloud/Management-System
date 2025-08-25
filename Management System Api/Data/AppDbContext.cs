using Management_System_Api.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Management_System_Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //  DbSets
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>(); // <-- Added
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<Invoice> Invoices => Set<Invoice>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Product config
            b.Entity<Product>(e =>
            {
                e.HasKey(x => x.ProductId);
                e.Property(x => x.ProductId).HasMaxLength(64);
                e.Property(x => x.Name).HasMaxLength(200).IsRequired();
                e.Property(x => x.Price).HasColumnType("decimal(18,2)");
                e.Property(x => x.IsActive).HasDefaultValue(true);

                // Relationship: Product belongs to Category
                e.HasOne(x => x.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //  Category config
            b.Entity<Category>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasMaxLength(100).IsRequired();
                e.HasIndex(x => x.Name).IsUnique(); // No duplicate categories
            });

            //  Sale config
            b.Entity<Sale>(e =>
            {
                e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                e.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");

                e.HasOne(x => x.Product)
                 .WithMany(p => p.Sales)
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.SoldBy)
                 .WithMany()
                 .HasForeignKey(x => x.SoldByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //  Invoice config
            b.Entity<Invoice>(e =>
            {
                e.HasIndex(x => x.InvoiceNumber).IsUnique();

                e.HasOne(x => x.Sale)
                 .WithOne(s => s.Invoice)
                 .HasForeignKey<Invoice>(x => x.SaleId);
            });
        }
    }
}
