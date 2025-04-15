namespace StealTheCatsApi.ThirdParty
{
    public class Breed
    {
        public string Temperament { get; set; } = string.Empty;

        public List<string> TemperamentItems
        {
            get
            {
                return !string.IsNullOrEmpty(Temperament)
                    ? Temperament
                        .Split(',')
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList()
                    : [];
            }
        }
    }
}
