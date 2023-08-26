using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web_Application.Models;

namespace Web_Application.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Books> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Specify column type for the Price property
            builder.Entity<Books>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)"); // Change the precision and scale as needed

            // Any additional configurations you might have
        }
    }
}
