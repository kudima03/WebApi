using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebAPI.Infrastructure.EntityConfigurations;
using WebAPI.Models;

namespace BooksAPI.Infrastructure
{
    public class BooksContext : DbContext
    {
        public BooksContext(DbContextOptions<BooksContext> options) : base(options)
        {
        }

        public DbSet<BookCard> BookCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookCardEntityTypeConfiguration());
        }


        public class BooksContextDesignFactory : IDesignTimeDbContextFactory<BooksContext>
        {
            public BooksContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<BooksContext>()
                    .UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=BooksServiceDatabase;Integrated Security=True");

                return new BooksContext(optionsBuilder.Options);
            }
        }

    }
}
