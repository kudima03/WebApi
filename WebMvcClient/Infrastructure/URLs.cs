using WebMvcClient.Models;

namespace WebMvcClient.Infrastructure
{
    public static class URLs
    {
        public static class Books
        {
            public static string GetBookByIdUrl(string hostUrl, int bookId) => $"{hostUrl}/books/GetById/?bookId={bookId}";
            public static string GetAllBooksUrl(string hostUrl) => $"{hostUrl}/books/all";
            public static string PostBookUrl(string hostUrl) => $"{hostUrl}/books/add";
            public static string UpdateBookUrl(string hostUrl) => $"{hostUrl}/books/edit";
            public static string DeleteBookUrl(string hostUrl) => $"{hostUrl}/books/delete";
        }

        public static class Pictures
        {
            public static string GetBookImageUrl(string hostUrl, int bookId) => $"{hostUrl}/Pictures/GetBookImage/?bookId{bookId}";
            public static string PostBookUrl(string hostUrl) => $"{hostUrl}/Pictures/AddImageToBook";
            public static string UpdateBookUrl(string hostUrl) => $"{hostUrl}/Pictures/EditBookImage";
            public static string DeleteBookUrl(string hostUrl) => $"{hostUrl}/Pictures/DeleteBookImage";
        }
    }
}
