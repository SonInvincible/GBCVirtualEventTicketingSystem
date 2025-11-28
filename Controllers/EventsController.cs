using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GBC_Ticketing.Data;
using GBC_Ticketing.Models;

namespace GBC_Ticketing.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchString, int? categoryId, DateTime? startDate, DateTime? endDate, string sortOrder, string availabilityFilter)
        {
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = categoryId;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["AvailabilityFilter"] = availabilityFilter;

            // Get categories for filter dropdown
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            var events = from e in _context.Events.Include(e => e.Category)
                         select e;

            // Search by title
            if (!String.IsNullOrEmpty(searchString))
            {
                events = events.Where(e => e.Title.Contains(searchString));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                events = events.Where(e => e.CategoryId == categoryId);
            }

            // Filter by date range
            if (startDate.HasValue)
            {
                events = events.Where(e => e.EventDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                events = events.Where(e => e.EventDate <= endDate.Value);
            }

            // Filter by availability status
            if (!String.IsNullOrEmpty(availabilityFilter))
            {
                switch (availabilityFilter.ToLower())
                {
                    case "available":
                        events = events.Where(e => e.AvailableTickets > 0);
                        break;
                    case "soldout":
                        events = events.Where(e => e.AvailableTickets == 0);
                        break;
                    case "low":
                        events = events.Where(e => e.AvailableTickets > 0 && e.AvailableTickets < 5);
                        break;
                }
            }

            // Sorting
            switch (sortOrder)
            {
                case "title_desc":
                    events = events.OrderByDescending(e => e.Title);
                    break;
                case "Date":
                    events = events.OrderBy(e => e.EventDate);
                    break;
                case "date_desc":
                    events = events.OrderByDescending(e => e.EventDate);
                    break;
                case "Price":
                    events = events.OrderBy(e => e.TicketPrice);
                    break;
                case "price_desc":
                    events = events.OrderByDescending(e => e.TicketPrice);
                    break;
                default:
                    events = events.OrderBy(e => e.Title);
                    break;
            }

            return View(await events.AsNoTracking().ToListAsync());
        }

        // GET: Events/Overview
        public async Task<IActionResult> Overview()
        {
            var totalEvents = await _context.Events.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var lowTicketEvents = await _context.Events
                .Include(e => e.Category)
                .Where(e => e.AvailableTickets < 5 && e.AvailableTickets > 0)
                .ToListAsync();

            ViewBag.TotalEvents = totalEvents;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.LowTicketCount = lowTicketEvents.Count;

            return View(lowTicketEvents);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,EventDate,TicketPrice,AvailableTickets,CategoryId")] Event eventModel)
        {
            if (ModelState.IsValid)
            {
                // FIX: Convert to UTC if not already
                if (eventModel.EventDate.Kind == DateTimeKind.Unspecified)
                {
                    eventModel.EventDate = DateTime.SpecifyKind(eventModel.EventDate, DateTimeKind.Utc);
                }
                else if (eventModel.EventDate.Kind == DateTimeKind.Local)
                {
                    eventModel.EventDate = eventModel.EventDate.ToUniversalTime();
                }
                
                _context.Add(eventModel);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", eventModel.CategoryId);
            return View(eventModel);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", eventModel.CategoryId);
            return View(eventModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,EventDate,TicketPrice,AvailableTickets,CategoryId")] Event eventModel)
        {
            if (id != eventModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // FIX: Convert to UTC if not already
                    if (eventModel.EventDate.Kind == DateTimeKind.Unspecified)
                    {
                        eventModel.EventDate = DateTime.SpecifyKind(eventModel.EventDate, DateTimeKind.Utc);
                    }
                    else if (eventModel.EventDate.Kind == DateTimeKind.Local)
                    {
                        eventModel.EventDate = eventModel.EventDate.ToUniversalTime();
                    }
                    
                    _context.Update(eventModel);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", eventModel.CategoryId);
            return View(eventModel);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.Events.FindAsync(id);
            if (eventModel != null)
            {
                _context.Events.Remove(eventModel);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Event deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}