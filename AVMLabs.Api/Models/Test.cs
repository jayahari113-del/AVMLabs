using System.ComponentModel.DataAnnotations;

namespace AVMLabs.Api.Models
{
    public class Test
    {
        public int TestId { get; set; }

        [Required, MaxLength(20)]
        public string TestCode { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string TestName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string SampleType { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Rate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
