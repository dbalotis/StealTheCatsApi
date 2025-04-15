using StolenCatsData.Entities;
using System.ComponentModel.DataAnnotations;

namespace StealTheCatsApi.Models
{
    public class CatResponse
    {
        public int Id { get; set; }

        public string CatId { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
