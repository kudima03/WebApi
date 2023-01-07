using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebAPI;
using WebAPI.Data.Interfaces;

namespace WebAPI.Grpc
{
    public class BooksService : Books.BooksBase
    {
        private readonly IBooksRepository _booksRepository;

        private readonly ILogger<BooksService> _logger;

        public BooksService(IBooksRepository booksRepository, ILogger<BooksService> logger)
        {
            _booksRepository = booksRepository;
            _logger = logger;
        }

        public override async Task<BookCreationReply> CreateBook(BookCreationRequest request, ServerCallContext context)
        {
            try
            {
                var id = await _booksRepository.CreateAsync(new Models.BookCard()
                {
                    Name = request.BookToCreate.Name,
                    BinaryPhoto = request.BookToCreate.BinaryPhoto.ToByteArray(),
                });
                return new BookCreationReply() { Id = id, Status = Status.Successfully };
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
                await _booksRepository.UpdateAsync(new Models.BookCard
                {
                    Id = request.BookNewVersion.Id,
                    Name = request.BookNewVersion.Name,
                    BinaryPhoto = request.BookNewVersion.BinaryPhoto.ToByteArray(),
                });
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
                if (request.Id.Count == 1)
                {
                    await _booksRepository.DeleteAsync(request.Id.First());
                }
                else
                {
                    await _booksRepository.DeleteRangeAsync(request.Id.ToArray());
                }
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
            foreach (var book in (await _booksRepository.GetAllAsync()).Take(request.Limit))
            {
                await responseStream.WriteAsync(new Book() { Id = book.Id, BinaryPhoto = ByteString.CopyFrom(book.BinaryPhoto), Name = book.Name});
            }
        }
    }
}