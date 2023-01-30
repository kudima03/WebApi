using WebMvcClient.Models;

namespace WebMvcClient.Infrastructure
{
    public static class URLs
    {
        public static string GetAllBooksUrl(string hostUrl) => $"{hostUrl}/books";
        public static string PostBookUrl(string hostUrl) => $"{hostUrl}/Post";
        public static string UpdateBookUrl(string hostUrl) => $"{hostUrl}/Put";
        public static string DeleteBookUrl(string hostUrl) => $"{hostUrl}/Delete";
    }
}
