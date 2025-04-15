using StolenCatsData.Entities;

namespace StealTheCatsApi.ThirdParty
{
    public class CatApiHttpClient
    {
        private readonly HttpClient _httpClient;

        public CatApiHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ICollection<CatApiItem>> GetRandomCats()
        {
            try
            {
                // The parameters are hardcoded here deliberately.
                // We do not want anyone editing the config file, and changing the parameters in a way that will cause the application not functioning as expected.
                var result = await _httpClient.GetFromJsonAsync<IList<CatApiItem>>("/v1/images/search?size=med&mime_types=jpg&format=json&has_breeds=true&order=RANDOM&page=0&limit=25");
                return result ?? [];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<byte[]> GetImageBytes(string url)
        {
            return await _httpClient.GetByteArrayAsync(url);
        }
    }
}
