using GBC_Ticketing.Data;

namespace GBCVirtualEventTicketingSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if database already has data
            if (context.Events.Any())
            {
                return; // DB has been seeded
            }

            // Additional initialization logic if needed
        }
    }
}