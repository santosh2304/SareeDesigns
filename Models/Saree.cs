using System.ComponentModel.DataAnnotations.Schema;

namespace SareeDesigns.Models
{
    public class Saree
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price    { get; set; }

        [NotMapped]
        public IFormFile? Photo { get; set; }
        public string? SavedUrl { get; set; }
        [NotMapped]
        public string? SignedUrl    { get; set; }
        public string? SavedFileName { get; set; }
    }
}
