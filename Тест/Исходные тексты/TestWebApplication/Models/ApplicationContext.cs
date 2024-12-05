using Microsoft.EntityFrameworkCore;

namespace TestWebApplication.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<product> product { get; set; }
        public DbSet<counterparty> counterparty { get; set; }
        public DbSet<document_header> document_header { get; set; }
        public DbSet<document_line> document_line { get; set; }
        public DbSet<product_stock> product_stock { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
