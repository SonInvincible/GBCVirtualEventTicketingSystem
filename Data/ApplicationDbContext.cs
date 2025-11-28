using Microsoft.EntityFrameworkCore;
using GBC_Ticketing.Models;

namespace GBC_Ticketing.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Event)
                .WithMany(e => e.PurchaseItems)
                .HasForeignKey(pi => pi.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data - USE UTC DATES
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Webinar", Description = "Online educational seminars" },
                new Category { Id = 2, Name = "Concert", Description = "Live music performances" },
                new Category { Id = 3, Name = "Workshop", Description = "Hands-on learning sessions" },
                new Category { Id = 4, Name = "Conference", Description = "Professional gatherings" }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Web Development Bootcamp",
                    EventDate = new DateTime(2025, 11, 15, 14, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 49.99m,
                    AvailableTickets = 100,
                    CategoryId = 1
                },
                new Event
                {
                    Id = 2,
                    Title = "Jazz Night Live",
                    EventDate = new DateTime(2025, 11, 20, 19, 30, 0, DateTimeKind.Utc),
                    TicketPrice = 75.00m,
                    AvailableTickets = 3,
                    CategoryId = 2
                },
                new Event
                {
                    Id = 3,
                    Title = "UI/UX Design Workshop",
                    EventDate = new DateTime(2025, 11, 25, 10, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 89.99m,
                    AvailableTickets = 50,
                    CategoryId = 3
                },
                new Event
                {
                    Id = 4,
                    Title = "Tech Innovation Conference 2025",
                    EventDate = new DateTime(2025, 12, 5, 9, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 199.99m,
                    AvailableTickets = 200,
                    CategoryId = 4
                },
                new Event
                {
                    Id = 5,
                    Title = "Rock Festival Virtual",
                    EventDate = new DateTime(2025, 11, 30, 18, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 59.99m,
                    AvailableTickets = 0,
                    CategoryId = 2
                },
                new Event
                {
                    Id = 6,
                    Title = "Digital Marketing Webinar",
                    EventDate = new DateTime(2025, 11, 18, 15, 0, 0, DateTimeKind.Utc),
                    TicketPrice = 29.99m,
                    AvailableTickets = 4,
                    CategoryId = 1
                }
            );
        }
    }
}