using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookService _service;

        public BookController(BookService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("get-by-key/{key}")]
        public IActionResult Get(string key)
        {
            return Ok(_service.GetByKey(key));
        }

        [HttpPost]
        public IActionResult Post([FromBody] BookModel book)
        {
           return Ok(_service.UpSert(book));
        }

        [HttpPut]
        public IActionResult Put([FromBody] BookModel book)
        {
            return Ok(_service.UpSert(book));
        }

        [HttpDelete("{key}")]
        public IActionResult Delete(string key)
        {
            _service.Remove(key);
            return NoContent();
        }
    }
}
