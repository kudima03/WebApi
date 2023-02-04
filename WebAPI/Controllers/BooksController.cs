using BooksAPI.ImageInfrastructure;
using BooksAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BooksContext _booksContext;

        private readonly ImageManager _imageManager;

        public BooksController(BooksContext booksRepository, ImageManager imageManager)
        {
            _booksContext = booksRepository;
            _imageManager = imageManager;
        }

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(typeof(IEnumerable<Models.BookCard>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<Models.BookCard>> GetAllBooks()
        {
            var books = await _booksContext.BookCards.AsNoTracking().ToArrayAsync();
            foreach (var book in books)
            {
                book.PictureUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Pictures/?bookId={book.Id}";
            }
            return books;
        }

        [HttpGet]
        [Route("GetById")]
        [ProducesResponseType(typeof(Models.BookCard), (int)HttpStatusCode.OK)]
        public async Task<Models.BookCard> GetById([FromQuery] int bookId)
        {
            var book = await _booksContext.BookCards.AsNoTracking().SingleOrDefaultAsync(x => x.Id == bookId);
            book.PictureUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/Pictures/?bookId={book.Id}";
            return book;
        }

        [HttpPost]
        [Route("add")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> Post([FromBody] Models.BookCard bookCard)
        {
            if (bookCard == null) return BadRequest("BookCard can't be null");

            await _booksContext.BookCards.AddAsync(bookCard);
            await _booksContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Route("edit")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> Put([FromBody] Models.BookCard bookCard)
        {
            if (bookCard == null) return BadRequest("BookCard can't be null.");

            var entity = await _booksContext.BookCards.FirstOrDefaultAsync(x => x.Id == bookCard.Id);

            if (entity == null) return NotFound("Entity to edit not found.");

            entity.Name = bookCard.Name;
            entity.Author = bookCard.Author;
            await _booksContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("delete")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Consumes("application/json")]
        public async Task<IActionResult> Delete([FromBody] int[] idArray)
        {
            if (idArray == null) return BadRequest("Input parameter can't be null.");

            if (idArray.Length == 0) return BadRequest("Invalid input: array length can't be 0.");

            var entitiesToDelete = from item in _booksContext.BookCards
                                   where idArray.Contains(item.Id)
                                   select item;

            if (await entitiesToDelete.CountAsync() == 0) return NotFound("No books was found.");

            entitiesToDelete.AsParallel().ForAll(x => _imageManager.DeleteBookImage(x.Id));
            _booksContext.BookCards.RemoveRange(entitiesToDelete);
            await _booksContext.SaveChangesAsync();
            return Ok();
        }
    }
}
