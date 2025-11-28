using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        [Required(ErrorMessage = "Guest name is required")]
        [StringLength(100)]
        [Display(Name = "Guest Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(200)]
        [Display(Name = "Guest Email")]
        public string GuestEmail { get; set; } = string.Empty;

        // Navigation property
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
    }
}