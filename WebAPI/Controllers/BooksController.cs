using CommonEntities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public async Task<List<BookCard>> Get()
        {
            return await FileManager.ReadAllAsync();
        }

        [HttpPost]
        public async Task<ActionResult<BookCard>> Post([FromBody]BookCard bookCard)
        {
            if (bookCard == null)
            {
                return BadRequest();
            }
            var res = await FileManager.CreateAsync(bookCard);
            return Ok(res);
        }

        [HttpPut]
        public async Task<ActionResult<BookCard>> Put([FromBody]BookCard bookCard)
        {
            if (bookCard == null)
            {
                return BadRequest();
            }
            var res = await FileManager.UpdateAsync(bookCard);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<ActionResult<BookCard>> Delete([FromBody]int[] idArray)
        {
            var res = await FileManager.DeleteRangeAsync(idArray);
            if (!res)
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
