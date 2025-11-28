using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GBC_Ticketing.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }

        [Required]
        public int PurchaseId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerTicket { get; set; }

        // Navigation properties
        [ForeignKey("PurchaseId")]
        public Purchase? Purchase { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}