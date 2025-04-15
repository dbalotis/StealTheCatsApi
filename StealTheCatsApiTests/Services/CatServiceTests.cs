using Xunit;
using Microsoft.EntityFrameworkCore;
using StealTheCatsApi.Services;
using StolenCatsData;
using StolenCatsData.Entities;
using Moq;
using StealTheCatsApi.ThirdParty;
using Moq.Protected;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StealTheCatsApiTests.Services
{
    public class CatsServiceTests
    {
        private readonly Mock<HttpClient> httpClientMock;
        private readonly CatApiHttpClient client;

        public CatsServiceTests()
        {
            httpClientMock = new Mock<HttpClient>();
            client = new CatApiHttpClient(httpClientMock.Object);
        }

        [Fact]
        public async Task FetchAndSaveCats_SavesOnlyNewCatsAndTags()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<StolenCatsDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new StolenCatsDbContext(options);

            // Existing cat in DB
            var existingCat = new Cat
            {
                CatId = "abc123",
                Width = 100,
                Height = 100,
                Image = [1, 2, 3],
            };
            context.Cats.Add(existingCat);
            await context.SaveChangesAsync();

            var fakeResponse = @"[
                {
                    ""id"": ""abc123"",
                    ""width"": 100,
                    ""height"": 100,
                    ""url"": ""http://thecatapi.com/image1.jpg"",
                    ""breeds"": [
                        {
                        ""temperament"": ""Tag1""
                        }
                    ]
                },
                {
                    ""id"": ""xyz789"",
                    ""width"": 150,
                    ""height"": 200,
                    ""url"": ""http://thecatapi.com/image2.jpg"",
                    ""breeds"": [
                        {
                        ""temperament"": ""Tag1, Tag2""
                        }
                    ]
                }
            ]";

            byte[] fakeImage = [10, 20, 30];

            var httpClientMock = GetMockedHttpClient(fakeResponse, fakeImage);

            var client = new CatApiHttpClient(httpClientMock);
            
            var service = new CatsService(context, client);

            // Act
            await service.FetchAndSaveCats();

            // Assert
            var allCats = await context.Cats.Include(c => c.Tags).ToListAsync();
            Assert.Equal(2, allCats.Count); // 1 existing + 1 new

            var newCat = allCats.Single(c => c.CatId == "xyz789");
            Assert.Equal(150, newCat.Width);
            Assert.Equal(200, newCat.Height);
            Assert.Equal([10, 20, 30], newCat.Image);
            Assert.Equal(2, newCat.Tags.Count);

            var allTags = await context.Tags.ToListAsync();
            Assert.Contains(allTags, t => t.Name == "Tag1");
            Assert.Contains(allTags, t => t.Name == "Tag2");
        }

        [Fact]
        public async Task GetCat_ReturnsCorrectCat()
        {
            var context = GetDbContext();
            var service = new CatsService(context, client);
            var expectedCat = context.Cats.First();

            var result = await service.GetCat(expectedCat.Id);

            Assert.NotNull(result);
            Assert.Equal(expectedCat.CatId, result!.CatId);
        }

        [Fact]
        public async Task GetCats_WhenNoTagFilter_ReturnsAllCats()
        {
            var context = GetDbContext();
            var service = new CatsService(context, client);

            var cats = await service.GetCats(page: 1, pageSize: 10);

            Assert.Equal(3, cats.Count);
        }

        [Fact]
        public async Task GetCats_WhenTagFilter_ReturnsFilteredCats()
        {
            var context = GetDbContext();
            var service = new CatsService(context, client);

            //var cats = await service.GetCats(page: 1, pageSize: 10, tag: "tag1");
            var cats = await service.GetCats(page: 1, pageSize: 10, tag: "tag1");

            Assert.Equal(2, cats.Count); // cat1 and cat3 have "tag1"
            Assert.All(cats, cat => Assert.Contains(cat.Tags, t => t.Name == "tag1"));
        }

        [Fact]
        public async Task GetCats_ReturnsPagedResults()
        {
            var context = GetDbContext();
            var service = new CatsService(context, client);

            var page1 = await service.GetCats(page: 1, pageSize: 2);
            var page2 = await service.GetCats(page: 2, pageSize: 2);

            Assert.Equal(2, page1.Count);
            Assert.Single(page2);
        }

        private StolenCatsDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<StolenCatsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new StolenCatsDbContext(options);

            // First, create and save tags
            var tag1 = new Tag { Name = "tag1" };
            var tag2 = new Tag { Name = "tag2" };
            var tag3 = new Tag { Name = "tag3" };

            context.Tags.AddRange(tag1, tag2, tag3);
            context.SaveChanges();

            // Then associate them with cats
            var cat1 = new Cat
            {
                CatId = Guid.NewGuid().ToString(),
                Width = 100,
                Height = 200,
                Image = new byte[] { 1, 2, 3 },
                Tags = new List<Tag> { tag1 }
            };

            var cat2 = new Cat
            {
                CatId = Guid.NewGuid().ToString(),
                Width = 150,
                Height = 250,
                Image = new byte[] { 4, 5, 6 },
                Tags = new List<Tag> { tag2 }
            };

            var cat3 = new Cat
            {
                CatId = Guid.NewGuid().ToString(),
                Width = 120,
                Height = 180,
                Image = new byte[] { 7, 8, 9 },
                Tags = new List<Tag> { tag1, tag3 }
            };

            context.Cats.AddRange(cat1, cat2, cat3);
            context.SaveChanges();

            return context;
        }

        private HttpClient GetMockedHttpClient(string jsonToReturn, byte[] fakeImage)
        {
            // Set up fake HttpMessageHandler
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains("/v1/images/search")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json"),
                });

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(".jpg")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(fakeImage)
                });

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://thecatapi.test.com/")
            };

            return httpClient;
        }
    }
}
