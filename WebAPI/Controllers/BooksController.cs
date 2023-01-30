using BooksAPI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
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

        public BooksController(BooksContext booksRepository)
        {
            _booksContext = booksRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Models.BookCard>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<Models.BookCard>> Get()
        {
            return await _booksContext.BookCards.AsNoTracking().ToListAsync();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] Models.BookCard bookCard)
        {
            if (bookCard == null) return BadRequest("Input parameter can't be null");
            try
            {
                await _booksContext.BookCards.AddAsync(bookCard);
                await _booksContext.SaveChangesAsync();
                return Ok();
            }
            catch (System.Exception)
            {
                return NoContent();
            }
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Put([FromBody] Models.BookCard bookCard)
        {
            if (bookCard == null) return BadRequest();
            try
            {
                _booksContext.BookCards.Update(bookCard);
                await _booksContext.SaveChangesAsync();
                return Ok();
            }
            catch (System.IO.InvalidDataException)
            {
                return BadRequest();
            }

        }

        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Delete([FromBody] int[] idArray)
        {
            if (idArray == null) return BadRequest("Input parameter can't be null");
            try
            {
                var entitiesToDelete = from item in _booksContext.BookCards
                                       where idArray.Contains(item.Id)
                                       select item;
                _booksContext.BookCards.RemoveRange(entitiesToDelete);
                await _booksContext.SaveChangesAsync();
                return Ok();
            }
            catch (System.IO.InvalidDataException)
            {
                return BadRequest("No one id was found");
            }
        }
    }
}
