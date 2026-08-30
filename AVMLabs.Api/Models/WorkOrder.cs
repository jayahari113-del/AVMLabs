using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVMLabs.Api.Models
{
    public class WorkOrder
    {
        [Key]
        public int WOId { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime WODate { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending / Completed

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        public ICollection<WorkOrderItem> Items { get; set; } = new List<WorkOrderItem>();
    }

    public class WorkOrderItem
    {
        [Key]
        public int WOItemId { get; set; }

        public int WOId { get; set; }
        public WorkOrder? WorkOrder { get; set; }

        public int TestId { get; set; }
        public Test? Test { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Rate { get; set; }

        [NotMapped]
        public decimal Amount => Quantity * Rate;
    }
}
