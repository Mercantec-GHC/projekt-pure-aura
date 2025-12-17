using System.ComponentModel.DataAnnotations;

namespace Pura.Services
{
    public class JewelryModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string Brand { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public string Gemstone { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
    }
}