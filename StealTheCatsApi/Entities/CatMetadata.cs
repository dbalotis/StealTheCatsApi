using StolenCatsData.Entities;
using System.ComponentModel.DataAnnotations;

namespace StealTheCatsApi.Entities
{
    public class CatMetadata
    {
        public int Id { get; set; }

        public string CatId { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
