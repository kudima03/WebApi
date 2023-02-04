using BooksAPI.ImageInfrastructure;
using BooksAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace BooksAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PicturesController : ControllerBase
    {
        private readonly BooksContext _bookContext;

        private readonly ImageManager _imageManager;

        public PicturesController(BooksContext bookContext, ImageManager imageManager)
        {
            _bookContext = bookContext;
            _imageManager = imageManager;
        }

        [HttpGet]
        [Route("GetBookImage")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBookImageAsync([FromQuery]int bookId)
        {
            if (bookId <= 0) return BadRequest();

            if (!await _bookContext.BookCards.AnyAsync(x => x.Id == bookId)) return NotFound();

            //MimeType as key, content as value
            var image = await _imageManager.GetImageAsync(bookId);

            return File(image.Value, image.Key);
        }

        [HttpPost]
        [Route("AddImageToBook")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddImageToBook([FromForm]int bookId, IFormFile formFile)
        {
            if (bookId <= 0) return BadRequest("Invalid bookId");

            var book = await _bookContext.BookCards.SingleOrDefaultAsync(x => x.Id == bookId);

            if (book == null) return NotFound("bookId doesn't exist");

            if (book.PictureFileName != null)
            {
                _imageManager.DeleteBookImage(book.Id);
            }

            book.PictureFileName = await _imageManager.CreateBookImageAsync(bookId, formFile);

            await _bookContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        [Route("EditBookImage")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateBookImage([FromForm]int bookId, IFormFile formFile)
        {
            if (bookId <= 0) return BadRequest("Invalid bookId.");

            var book = await _bookContext.BookCards.SingleOrDefaultAsync(x => x.Id == bookId);

            if (book == null) return NotFound("bookId doesn't exist.");

            if (book.PictureFileName == null) return BadRequest("Book doesn't have image to update.");

            book.PictureFileName = await _imageManager.UpdateBookImageAsync(bookId, formFile);
            await _bookContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("DeleteBookImage")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteBookImage([FromQuery] int bookId)
        {
            if (bookId <= 0) return BadRequest("Invalid bookId.");

            var book = await _bookContext.BookCards.SingleOrDefaultAsync(x => x.Id == bookId);

            if (book == null) return NotFound("bookId doesn't exist.");

            if (book.PictureFileName == null) return BadRequest("Book doesn't have image to delete.");

            _imageManager.DeleteBookImage(bookId);

            book.PictureFileName = null;

            await _bookContext.SaveChangesAsync();

            return Ok();
        }
    }
}
