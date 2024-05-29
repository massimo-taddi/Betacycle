using System.ComponentModel.DataAnnotations;

namespace BetaCycleAPI.Models
{
    public class ReviewDataForm
    {
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(500), MinLength(4)]
        public string BodyDescription { get; set; } = null!;

        [Required]
        public byte Rating { get; set; }

        public DateTime? ReviewDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? CustomerName { get; set; }
    }
}
