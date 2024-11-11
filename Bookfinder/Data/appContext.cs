using Microsoft.EntityFrameworkCore;
using Bookfinder.Models;

namespace Bookfinder.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }

        // DbSet para Books
        public DbSet<Book> Books { get; set; }

        // DbSet para Users
        public DbSet<User> Users { get; set; }

        public DbSet<FavoriteBook> FavoriteBooks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
        }
    }
}
