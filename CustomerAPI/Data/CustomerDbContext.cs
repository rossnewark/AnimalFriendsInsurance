using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }
        
        public DbSet<Customer> Customers { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
                
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PolicyReferenceNumber).IsRequired().HasMaxLength(9);
                
                entity.HasIndex(e => e.PolicyReferenceNumber).IsUnique();
            });
        }
    }
}