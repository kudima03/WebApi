using BooksAPI.ImageInfrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Models;

namespace BooksAPI.Infrastructure
{
    public class BooksContextSeed
    {
        public async Task SeedAsync(BooksContext context, IWebHostEnvironment env, ILogger<BooksContextSeed> logger)
        {
            if (!context.BookCards.Any())
            {
                await context.BookCards.AddRangeAsync(await GetBookCardsFromFileAsync(env, logger));
                await context.SaveChangesAsync();
            }
        }

        private async Task<IEnumerable<BookCard>> GetBookCardsFromFileAsync(IWebHostEnvironment env, ILogger<BooksContextSeed> logger)
        {
            var jsonBooksFile = Path.Combine(env.ContentRootPath, "Setup", "books.json");
            if (!File.Exists(jsonBooksFile))
            {
                logger.LogError("Setup file not found in " + jsonBooksFile);
                return new List<BookCard>();
            }
            try
            {
                return JsonSerializer.Deserialize<List<BookCard>>(await File.ReadAllTextAsync(jsonBooksFile));
            }
            catch (System.Exception)
            {
                logger.LogError("Unable to deserialize setup file " + jsonBooksFile);
                return new List<BookCard>();
            }
        }
    }
}
