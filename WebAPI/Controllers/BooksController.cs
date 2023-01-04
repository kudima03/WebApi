using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookCard>), (int)HttpStatusCode.OK)]
        public async Task<List<BookCard>> Get()
        {
            return await FileManager.ReadAllAsync();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]BookCard bookCard)
        {
            if (bookCard == null) return BadRequest("Input parameter can't be null");
            try
            {
                await FileManager.CreateAsync(bookCard);
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
        public async Task<IActionResult> Put([FromBody]BookCard bookCard)
        {
            if (bookCard == null) return BadRequest();
            try
            {
                await FileManager.UpdateAsync(bookCard);
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
        public async Task<IActionResult> Delete([FromBody]int[] idArray)
        {
            if (idArray == null) return BadRequest("Input parameter can't be null");
            try
            {
                await FileManager.DeleteRangeAsync(idArray);
                return Ok();
            }
            catch (System.IO.InvalidDataException)
            {
                return BadRequest("No one id was found");
            }
        }
    }
}
