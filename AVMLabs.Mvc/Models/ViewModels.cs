using System.ComponentModel.DataAnnotations;

namespace AVMLabs.Mvc.Models
{
    public class ClientListItemVm
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class ClientListPageVm
    {
        public List<ClientListItemVm> Clients { get; set; } = new();
        public string? SearchTerm { get; set; }
    }

    public class WorkOrderRowVm
    {
        public DateTime WODate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class ClientDetailsVm
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public List<WorkOrderRowVm> WorkOrders { get; set; } = new();
    }

    public class ClientCreateVm
    {
        [Required, Display(Name = "Client Name")]
        public string ClientName { get; set; } = string.Empty;

        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        public string? Phone { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? City { get; set; }
        public string? Country { get; set; }
    }

    public class TestOptionVm
    {
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }

    public class WorkOrderEntryVm
    {
        public List<ClientListItemVm> Clients { get; set; } = new();
        public List<TestOptionVm> Tests { get; set; } = new();
        public int? ClientId { get; set; }
        public List<WorkOrderLineVm> Lines { get; set; } = new();
        public string? Message { get; set; }
        public bool Success { get; set; }
    }

    public class WorkOrderLineVm
    {
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount => Quantity * Rate;
    }
}
