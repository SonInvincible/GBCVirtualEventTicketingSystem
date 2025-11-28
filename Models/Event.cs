using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Event title is required")]
        [StringLength(200)]
        [Display(Name = "Event Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event Date")]
        [DataType(DataType.DateTime)]
        public DateTime EventDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Ticket price must be a positive value")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Ticket Price")]
        public decimal TicketPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available tickets must be non-negative")]
        [Display(Name = "Available Tickets")]
        public int AvailableTickets { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();

        // Computed property for low ticket alert
        [NotMapped]
        public bool IsLowTicket => AvailableTickets < 5;

        [NotMapped]
        public bool IsSoldOut => AvailableTickets == 0;
    }
}