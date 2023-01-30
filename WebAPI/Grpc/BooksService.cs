using BooksAPI.Infrastructure;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Grpc
{
    public class BooksService : Books.BooksBase
    {
        private readonly BooksContext _booksContext;

        private readonly ILogger<BooksService> _logger;

        public BooksService(BooksContext booksRepository, ILogger<BooksService> logger)
        {
            _booksContext = booksRepository;
            _logger = logger;
        }

        public override async Task<BookCreationReply> CreateBook(BookCreationRequest request, ServerCallContext context)
        {
            try
            {
                var entity = await _booksContext.BookCards.AddAsync(new Models.BookCard()
                {
                    Name = request.BookToCreate.Name,
                    BinaryPhoto = request.BookToCreate.BinaryPhoto.ToByteArray(),
                });
                await _booksContext.SaveChangesAsync();
                return new BookCreationReply() { Id = entity.Entity.Id, Status = Status.Successfully };
            }
            catch (System.Exception)
            {
                return new BookCreationReply() { Id = 0, Status = Status.Error };
            }
        }

        public override async Task<BookEditReply> EditBook(BookEditRequest request, ServerCallContext context)
        {
            try
            {
                _booksContext.BookCards.Update(new Models.BookCard
                {
                    Id = request.BookNewVersion.Id,
                    Name = request.BookNewVersion.Name,
                    BinaryPhoto = request.BookNewVersion.BinaryPhoto.ToByteArray(),
                });
                await _booksContext.SaveChangesAsync();
                return new BookEditReply() { Status = Status.Successfully };
            }
            catch (InvalidDataException)
            {
                return new BookEditReply() { Status = Status.DataNotFound };
            }
            catch (System.Exception)
            {
                return new BookEditReply { Status = Status.Error };
            }
        }

        public override async Task<BookDeleteReply> DeleteBook(BookDeleteRequest request, ServerCallContext context)
        {
            if (request.Id.Count == 0) return new BookDeleteReply() { Status = Status.DataNotFound };
            try
            {
                var entitiesToDelete = from item in _booksContext.BookCards
                                       where request.Id.Contains(item.Id)
                                       select item;
                _booksContext.BookCards.RemoveRange(entitiesToDelete);
                await _booksContext.SaveChangesAsync();
                return new BookDeleteReply() { Status = Status.Successfully };
            }
            catch (InvalidDataException)
            {
                return new BookDeleteReply() { Status = Status.DataNotFound };
            }
            catch (System.Exception)
            {
                return new BookDeleteReply() { Status = Status.Error };
            }
        }

        public override async Task GetAllBooks(BooksGetRequest request, IServerStreamWriter<Book> responseStream, ServerCallContext context)
        {
            await foreach (var book in  _booksContext.BookCards.Take(request.Limit).AsAsyncEnumerable())
            {
                await responseStream.WriteAsync(new Book() { Id = book.Id, BinaryPhoto = ByteString.CopyFrom(book.BinaryPhoto), Name = book.Name});
            }
        }
    }
}