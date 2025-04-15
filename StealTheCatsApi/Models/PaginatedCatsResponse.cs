namespace StealTheCatsApi.Models
{
    public class PaginatedCatsResponse
    {
        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public int Page {  get; set; }

        public required ICollection<CatResponse> Cats { get; set; }
    }
}
