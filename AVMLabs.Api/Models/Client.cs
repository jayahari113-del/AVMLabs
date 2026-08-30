using System.ComponentModel.DataAnnotations;

namespace AVMLabs.Api.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required, MaxLength(150)]
        public string ClientName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactPerson { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
