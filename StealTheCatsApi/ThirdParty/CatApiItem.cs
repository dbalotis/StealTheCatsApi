using System.ComponentModel.DataAnnotations;

namespace StealTheCatsApi.ThirdParty
{
    public class CatApiItem
    {
        public required string Id { get; set; }

        public required int Width { get; set; }

        public required int Height { get; set; }

        public required string Url { get; set; }

        public required List<Breed> Breeds { get; set; }

        public List<string> Tags
        {
            get
            {
                var tags = new List<string>();
                foreach (var breed in Breeds) 
                {
                    tags.AddRange(breed.TemperamentItems);
                }
                return tags;
            }
        }
    }
}
