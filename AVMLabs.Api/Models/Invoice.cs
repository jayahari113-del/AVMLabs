using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVMLabs.Api.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending / Paid
    }
}
