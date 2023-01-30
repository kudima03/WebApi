using WebMvcClient.Models;

namespace WebMvcClient.Services
{
    public interface IBooksHttpClient
    {
        Task<List<BookCard>> GetBooksAsync();
        Task PostBookAsync(BookCard book);
        Task UpdateBookAsync(BookCard book);
        Task DeleteBooksAsync(IEnumerable<BookCard> books);
    }
}
