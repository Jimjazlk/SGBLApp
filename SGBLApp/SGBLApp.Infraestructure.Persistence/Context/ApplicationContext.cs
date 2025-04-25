using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SGBLApp.Core.Domain.Entities;

namespace SGBLApp.Infraestructure.Persistence.Context
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> contextOptions) : base(contextOptions) { }

        #region DbSets
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookPopularity> BookPopularity { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<BookFeedback> BookFeedbacks { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Configuración base de Identity

            #region Tables
            builder.Entity<Author>().ToTable("Author");
            builder.Entity<Book>().ToTable("Book");
            builder.Entity<BookPopularity>().ToTable("BookPopularity");
            builder.Entity<Genre>().ToTable("Genre");
            builder.Entity<Library>().ToTable("Library");
            builder.Entity<Loan>().ToTable("Loan");
            builder.Entity<Notification>().ToTable("Notification");
            builder.Entity<Reservation>().ToTable("Reservation");
            #endregion

            // Configuración global para evitar eliminación en cascada
            foreach (var relationship in builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            #region ApplicationUser
            // Configuración de ApplicationUser
            builder.Entity<ApplicationUser>(b =>
            {
                // Relación con Library
                b.HasOne(u => u.Library)
                    .WithMany(l => l.Users)
                    .HasForeignKey(u => u.LibraryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación 1:N con Loan
                b.HasMany(u => u.Loans)
                    .WithOne(l => l.User)
                    .HasForeignKey(l => l.UserId);

                // Relación 1:N con Reservation
                b.HasMany(u => u.Reservations)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId);

                // Relación 1:N con Notification
                b.HasMany(u => u.Notifications)
                    .WithOne(n => n.User)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region Book
            // Configuración de Book
            builder.Entity<Book>(b =>
            {
                b.HasKey(x => x.BookId);
                b.HasIndex(x => x.ISBN).IsUnique();

                // Relación 1:1 con BookPopularity
                b.HasOne(x => x.BookPopularity)
                    .WithOne(x => x.Book)
                    .HasForeignKey<BookPopularity>(x => x.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Library)
                    .WithMany(x => x.Books)
                    .HasForeignKey(x => x.LibraryId);

                b.HasOne(x => x.Author)
                    .WithMany(x => x.Books)
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.PrimaryGenre)
                    .WithMany(x => x.PrimaryBooks)
                    .HasForeignKey(x => x.PrimaryGenreId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.SecondaryGenre)
                    .WithMany(x => x.SecondaryBooks)
                    .HasForeignKey(x => x.SecondaryGenreId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.Property(x => x.Copies)
                    .IsRequired()
                    .HasDefaultValue(1);

                b.Property(x => x.AvailableCopies)
                    .IsRequired()
                    .HasDefaultValue(1);
            });
            #endregion

            #region BookPopularity
            // Configuración de BookPopularity
            builder.Entity<BookPopularity>(bp =>
            {
                bp.HasKey(x => x.BookId);
            });
            #endregion

            #region Loan
            // Configuración de Loan
            builder.Entity<Loan>(b =>
            {
                b.HasKey(l => l.LoanId);

                // Relación con Book
                b.HasOne(l => l.Book)
                    .WithMany(b => b.Loans)
                    .HasForeignKey(l => l.BookId);
            });
            #endregion

            #region Reservation
            builder.Entity<Reservation>(b =>
            {
                b.HasOne(r => r.User)
                    .WithMany(u => u.Reservations)
                    .HasForeignKey(r => r.UserId);

                b.HasOne(r => r.Book)
                    .WithMany(b => b.Reservations)
                    .HasForeignKey(r => r.BookId);

                b.Property(r => r.Status)
                    .HasConversion<string>();
            });
            #endregion
        }
    }
}
