using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GBC_Ticketing.Data;
using GBC_Ticketing.Models;

namespace GBC_Ticketing.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchasesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchases/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            if (@event.AvailableTickets == 0)
            {
                TempData["Error"] = "Sorry, this event is sold out!";
                return RedirectToAction("Index", "Events");
            }

            return View(@event);
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, int quantity, string guestName, string guestEmail)
        {
            var @event = await _context.Events.FindAsync(eventId);

            if (@event == null)
            {
                return NotFound();
            }

            // Validate quantity
            if (quantity < 1)
            {
                ModelState.AddModelError("", "Quantity must be at least 1");
                return View(@event);
            }

            if (quantity > @event.AvailableTickets)
            {
                ModelState.AddModelError("", $"Only {@event.AvailableTickets} tickets available");
                return View(@event);
            }

            if (string.IsNullOrWhiteSpace(guestName) || string.IsNullOrWhiteSpace(guestEmail))
            {
                ModelState.AddModelError("", "Guest name and email are required");
                return View(@event);
            }

            // Create purchase
            var purchase = new Purchase
            {
                PurchaseDate = DateTime.UtcNow,
                GuestName = guestName.Trim(),
                GuestEmail = guestEmail.Trim(),
                TotalCost = @event.TicketPrice * quantity
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Create purchase item
            var purchaseItem = new PurchaseItem
            {
                PurchaseId = purchase.Id,
                EventId = eventId,
                Quantity = quantity,
                PricePerTicket = @event.TicketPrice
            };

            _context.PurchaseItems.Add(purchaseItem);

            // Update available tickets
            @event.AvailableTickets -= quantity;
            _context.Update(@event);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Confirmation), new { id = purchase.Id });
        }

        // GET: Purchases/Confirmation/5
        public async Task<IActionResult> Confirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Event)
                .ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
    }
}