using System.Text;
using System.Text.Json;
using WebMvcClient.Infrastructure;
using WebMvcClient.Models;

namespace WebMvcClient.Services
{
    public class BooksHttpClient : IBooksHttpClient
    {
        private readonly HttpClient _httpClient;

        private readonly string _booksApiUrl;

        public BooksHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _booksApiUrl = configuration.GetValue<string>("BooksApiUrl");
        }

        public async Task DeleteBooksAsync(IEnumerable<BookCard> books)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(URLs.Books.DeleteBookUrl(_booksApiUrl)),
                Content = new StringContent(JsonSerializer.Serialize(books.Select(x=>x.Id)), Encoding.UTF8, "application/json"),
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<BookCard>> GetBooksAsync()
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(URLs.Books.GetAllBooksUrl(_booksApiUrl)),
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var a = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<BookCard>>(a, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task PostBookAsync(BookCard book)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(URLs.Books.PostBookUrl(_booksApiUrl)),
                Content = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json"),
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateBookAsync(BookCard book)
        {
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(URLs.Books.UpdateBookUrl(_booksApiUrl)),
                Content = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json")
            };
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}
