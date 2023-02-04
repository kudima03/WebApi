using BooksAPI.Controllers;
using BooksAPI.ImageInfrastructure;
using BooksAPI.Infrastructure;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
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

        private readonly ImageManager _imageManager;

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<BooksService> _logger;

        public BooksService(BooksContext booksRepository,
                        ImageManager imageManager,
                        ILogger<BooksService> logger,
                        IWebHostEnvironment webHostEnvironment)
        {
            _booksContext = booksRepository;
            _logger = logger;
            _imageManager = imageManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public override async Task<BookCreationReply> CreateBook(BookCreationRequest request, ServerCallContext context)
        {
            try
            {
                var entityEntry = await _booksContext.BookCards.AddAsync(new BookCard()
                {
                    Name = request.BookToCreate.Name,
                    Author = request.BookToCreate.Author,
                });

                entityEntry.Entity.PictureFileName = await _imageManager.CreateBookImageAsync(
                                                entityEntry.Entity.Id,
                                                request.BookToCreate.BinaryPhoto.ToArray(),
                                                request.BookToCreate.PhotoExtension);

                entityEntry.Entity.PictureUri = _webHostEnvironment.EnvironmentName + $"/Pictures/{entityEntry.Entity.Id}/pic";
                await _booksContext.SaveChangesAsync();
                return new BookCreationReply() { Id = entityEntry.Entity.Id, Status = Status.Successfully };
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
                var entityEntry = _booksContext.BookCards.Update(new BookCard()
                {
                    Name = request.BookNewVersion.Name,
                    Author = request.BookNewVersion.Author,
                });

                await _imageManager.UpdateBookImageAsync(
                                                entityEntry.Entity.Id,
                                                request.BookNewVersion.BinaryPhoto.ToArray(),
                                                request.BookNewVersion.PhotoExtension);

                entityEntry.Entity.PictureUri = _webHostEnvironment.EnvironmentName + $"/Pictures/{entityEntry.Entity.Id}/pic";
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
                entitiesToDelete.AsParallel().ForAll(x => _imageManager.DeleteBookImage(x.Id));
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
            await foreach (var book in _booksContext.BookCards.Take(request.Limit).AsAsyncEnumerable())
            {
                var bookBuf = new Book()
                {
                    Id = book.Id,
                    Name = book.Name,
                    Author = book.Author,
                    ImageFileName = book.PictureFileName,
                    ImageUri = book.PictureUri
                };
                await responseStream.WriteAsync(bookBuf);
            }
        }
    }
}