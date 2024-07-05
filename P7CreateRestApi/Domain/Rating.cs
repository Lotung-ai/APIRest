using System.ComponentModel.DataAnnotations;

namespace P7CreateRestApi.Domain
{
    public class Rating
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "MoodysRating is required")]
        public string MoodysRating { get; set; }

        [Required(ErrorMessage = "SandPRating is required")]
        public string SandPRating { get; set; }

        [Required(ErrorMessage = "FitchRating is required")]
        public string FitchRating { get; set; }

        [Range(0, 100, ErrorMessage = "OrderNumber must be between 0 and 100")]
        public byte? OrderNumber { get; set; }
    }
}
