using System.ComponentModel.DataAnnotations;

namespace AVMLabs.Api.DTOs
{
    public class WorkOrderListDto
    {
        public int WOId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime WODate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class WorkOrderItemCreateDto
    {
        [Required]
        public int TestId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }

    public class WorkOrderCreateDto
    {
        [Required(ErrorMessage = "ClientId is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "At least one work order item is required.")]
        [MinLength(1, ErrorMessage = "At least one work order item is required.")]
        public List<WorkOrderItemCreateDto> Items { get; set; } = new();
    }

    public class WorkOrderResponseDto
    {
        public int WOId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime WODate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<WorkOrderItemResponseDto> Items { get; set; } = new();
    }

    public class WorkOrderItemResponseDto
    {
        public int WOItemId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }

    public class ClientSummaryDto
    {
        public string ClientName { get; set; } = string.Empty;
        public decimal TotalWorkOrderAmount { get; set; }
    }
}
