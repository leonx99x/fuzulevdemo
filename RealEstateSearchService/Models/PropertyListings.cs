
using System.ComponentModel.DataAnnotations;

namespace RealEstateSearchService.Model
{
    public class PropertyListing
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Location { get; set; }
        [MaxLength(100)]
        [Required]
        [RegularExpression(@"^\d+(\.\d+)?$")]
        public float Price { get; set; }
        [MaxLength(20)]
        [Required]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }
        [MaxLength(50)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(100)]
        [Required]
        public required string PropertyType { get; set; }
        [MaxLength(250)]
        [Required]
        public required string Description { get; set; }
        [MaxLength(250)]
        [Url]
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

}
