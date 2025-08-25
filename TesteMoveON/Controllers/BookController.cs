using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TesteMoveON.Data;
using TesteMoveON.Models;

namespace TesteMoveON
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookController> _logger;
        
        public BookController(AppDbContext context, ILogger<BookController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        // GET: api/<BookController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            _logger.LogInformation("Get foi chamadooooooo");
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(AppDbContext context, int id)
        {
            var book = await context.Books.FindAsync(id);
            return Ok(book);
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Book book)
        {
            if (book == null)
            {
                return BadRequest("Book cannot be null.");
            }

            //book = new Book();
            
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Book books)
        {
            var book = await _context.Books.FindAsync(id);
            
            if (book == null)
            {
                return BadRequest("Book cannot be null.");
            }
            
            book.Title = books.Title;
            book.Author = books.Author;
            book.Genre = books.Genre;
            
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            
            _context.Books.Remove(book);
            
            await _context.SaveChangesAsync();
            return Ok("Book deleted successfully.");
        }
    }
}
