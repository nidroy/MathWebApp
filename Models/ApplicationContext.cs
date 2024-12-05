using Microsoft.EntityFrameworkCore;

namespace MathWebApp.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ArithmeticOperation> ArithmeticOperations { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<List<string>> operations = new()
            {
                new() { "Сложение", "2", "3", "5"},
                new() { "Вычитание", "5", "3", "2" },
                new() { "Умножение", "2", "3", "6" }
            };

            for (int i = 0; i < operations.Count; i++)
            {
                ArithmeticOperation operation = new()
                {
                    ID = i,
                    Operation = operations[i][0],
                    A = int.Parse(operations[i][1]),
                    B = int.Parse(operations[i][2]),
                    Res = int.Parse(operations[i][3]),
                };

                modelBuilder.Entity<ArithmeticOperation>().HasData(operation);
            }
        }
    }
}
