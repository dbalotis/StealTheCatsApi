using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StealTheCatsApi.Models;
using StealTheCatsApi.Services;
using StolenCatsData.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StealTheCatsApi.Controllers
{
    [ApiController]
    public class CatsController : ControllerBase
    {
        private readonly CatsService _catsService;
        public CatsController(CatsService catsService)
        {
            _catsService = catsService;
        }

        /// <summary>
        /// Starts a background fetch cats process, that saves the cat images and data in the DB
        /// </summary>
        /// <returns>The background jobId to track when the process has finished</returns>
        [HttpPost("api/cats/fetch")]
        public string FetchCats()
        {
            var jobId = BackgroundJob.Enqueue(() => _catsService.FetchAndSaveCats());
            return jobId;
        }

        /// <summary>
        /// Returns a cat with its info from our DB
        /// </summary>
        /// <param name="id">The cat id</param>
        /// <returns>CatResponse</returns>
        [HttpGet("api/cats/{id}")]
        public async Task<CatResponse?> GetCat(int id)
        {
            var cat = await _catsService.GetCat(id);

            if (cat == null)
            {
                return null;
            }

            return new CatResponse
            {
                Id = cat.Id,
                CatId = cat.CatId,
                Height = cat.Height,
                Width = cat.Width,
                Tags = cat.Tags.Select(x => x.Name).ToList()
            };
        }
        /// <summary>
        /// Returns the image of a cat
        /// </summary>
        /// <param name="id">The cat id</param>
        /// <returns></returns>
        [HttpGet("api/cats/{id}/image")]
        public async Task<IActionResult> GetCatImage(int id)
        {
            var image = await _catsService.GetCatImage(id);
            if (image == null || image == null || image.Length == 0)
                return NotFound();

            return File(image, "image/jpeg");
        }

        /// <summary>
        /// Returns a list of cats with its info from our DB
        /// </summary>
        /// <param name="page">The page we are requesting</param>
        /// <param name="pageSize">The size of the page</param>
        /// <param name="tag">Optional. Searching with a specific tag</param>
        /// <returns>PaginatedCatsResponse</returns>
        [HttpGet("api/cats/")]
        public async Task<PaginatedCatsResponse> GetCats([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string tag = "")
        {
            var cats = await _catsService.GetCats(page, pageSize, tag);

            var totalCount = await _catsService.GetCatsTotalCount();

            var pageResult = cats.Select(x => new CatResponse
            {
                Id = x.Id,
                CatId = x.CatId,
                Height = x.Height,
                Width = x.Width,
                Tags = x.Tags.Select(x => x.Name).ToList()
            }).ToList();

            return new PaginatedCatsResponse
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Cats = pageResult
            };
        }
    }
}
