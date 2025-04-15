using System.ComponentModel.DataAnnotations;

namespace StolenCatsData.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tag name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Tag name can be up to 50 characters.")]
        public string Name { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}
