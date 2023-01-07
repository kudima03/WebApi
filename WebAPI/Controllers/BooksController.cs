using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WebAPI.Data.Implementations;
using WebAPI.Data.Interfaces;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {

        private readonly IBooksRepository _booksRepository;

        public BooksController(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookCard>), (int)HttpStatusCode.OK)]
        public async Task<IEnumerable<BookCard>> Get()
        {
            return await _booksRepository.GetAllAsync();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] BookCard bookCard)
        {
            if (bookCard == null) return BadRequest("Input parameter can't be null");
            try
            {
                await _booksRepository.CreateAsync(bookCard);
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
        public async Task<IActionResult> Put([FromBody] BookCard bookCard)
        {
            if (bookCard == null) return BadRequest();
            try
            {
                await _booksRepository.UpdateAsync(bookCard);
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
                await _booksRepository.DeleteRangeAsync(idArray);
                return Ok();
            }
            catch (System.IO.InvalidDataException)
            {
                return BadRequest("No one id was found");
            }
        }
    }
}
