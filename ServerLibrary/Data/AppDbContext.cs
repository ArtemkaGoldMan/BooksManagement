using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary.Entities;

namespace ServerLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Borrow> Borrows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book entity configuration
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(b => b.Title)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(b => b.Author)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(b => b.Genre)
                    .HasMaxLength(50);

                entity.Property(b => b.Price)
                    .HasPrecision(10, 2) // Precision 10, scale 2
                    .IsRequired();

                entity.Property(b => b.PublishedDate)
                    .IsRequired();
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.PasswordHash)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);

            // Borrow entity configuration
            modelBuilder.Entity<Borrow>(entity =>
            {
                entity.HasOne(b => b.Book)
                    .WithMany(book => book.Borrows)
                    .HasForeignKey(b => b.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.User)
                    .WithMany(user => user.Borrows)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(b => b.BorrowDate)
                    .IsRequired();

                entity.Property(b => b.ReturnDate)
                    .IsRequired(false); // Nullable for ongoing borrows
            });
        }
    }
}
