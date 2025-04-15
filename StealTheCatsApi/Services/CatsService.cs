using Microsoft.EntityFrameworkCore;
using StealTheCatsApi.Entities;
using StealTheCatsApi.ThirdParty;
using StolenCatsData;
using StolenCatsData.Entities;

namespace StealTheCatsApi.Services
{
    public class CatsService
    {
        private readonly StolenCatsDbContext _context;
        private readonly CatApiHttpClient _client;

        public CatsService(StolenCatsDbContext context, CatApiHttpClient client) 
        {
            _context = context;
            _client = client;
        }

        public async Task FetchAndSaveCats()
        {
            //Get 25 random cats
            var stolenCats = await _client.GetRandomCats();
            var stolenCatsIds = stolenCats.Select(x => x.Id).ToList();

            //Select any existing ones from the DB
            var existingCatIds = await _context.Cats
                .Where(c => stolenCatsIds.Contains(c.CatId))
                .Select(x => x.CatId)
                .ToListAsync();

            //Filter out the existing ones
            var newStolenCats = stolenCats.Where(x => !existingCatIds.Contains(x.Id));

            foreach (var cat in newStolenCats)
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var tags = await SaveAndGetTags(cat.Tags);
                    var entity = new Cat
                    {
                        CatId = cat.Id,
                        Width = cat.Width,
                        Height = cat.Height,
                        Tags = tags,
                        Image = await _client.GetImageBytes(cat.Url)
                    };
                    _context.Cats.Add(entity);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
        }

        private async Task<ICollection<Tag>> SaveAndGetTags(List<string> tagNames)
        {
            if (tagNames.Count == 0)
                return [];

            var existingTags = await _context.Tags.Where(t => tagNames.Contains(t.Name)).ToListAsync();
            var existingTagNames = existingTags.Select(t => t.Name).ToList();

            // Find which tags are missing
            var missingTagNames = tagNames
                .Where(name => !existingTagNames.Contains(name))
                .ToList();

            // Create new Tag entities
            var newTags = missingTagNames
                .Select(name => new Tag { Name = name })
                .ToList();

            if (newTags.Any())
            {
                _context.Tags.AddRange(newTags);
                await _context.SaveChangesAsync();
            }

            return existingTags.Concat(newTags).ToList();
        }

        public async Task<CatMetadata?> GetCat(int id) => 
            await _context.Cats
                .Where(x => x.Id == id)
                .Include(c => c.Tags)
                .Select(x => new CatMetadata
                {
                    Id = x.Id,
                    CatId = x.CatId,
                    Created = x.Created,
                    Height = x.Height,
                    Tags = x.Tags,
                    Width = x.Width
                })
                .SingleOrDefaultAsync();

        public async Task<byte[]?> GetCatImage(int id) =>
            await _context.Cats
                .Where(x => x.Id == id)
                .Include(c => c.Tags)
                .Select(x => x.Image)
                .SingleOrDefaultAsync();

        public async Task<ICollection<CatMetadata>> GetCats(int page, int pageSize, string tag = "")
        {
            var query = _context.Cats
                .OrderBy(s => s.Id)
                .Include(c => c.Tags)
                .AsQueryable();

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(c => c.Tags.Any(t => t.Name == tag));
            }

            //Console.WriteLine(query.ToQueryString());

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CatMetadata
                {
                    Id = x.Id,
                    CatId = x.CatId,
                    Created = x.Created,
                    Height = x.Height,
                    Tags = x.Tags,
                    Width = x.Width
                })
                .ToListAsync();
        }

        public async Task<int> GetCatsTotalCount()
        {
            return await _context.Cats.CountAsync();
        }
    }
}
