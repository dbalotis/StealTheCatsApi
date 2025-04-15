using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StolenCatsData.Entities
{
    public class Cat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CatId is required.")]
        [StringLength(100)]
        public string CatId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Width is required.")]
        public int Width { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        public int Height { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public byte[] Image { get; set; } = [];

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
