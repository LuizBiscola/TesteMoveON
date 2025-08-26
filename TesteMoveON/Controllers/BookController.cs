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
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            _logger.LogInformation("Iniciando busca de todos os livros às {Timestamp}", DateTime.UtcNow);
            
            try
            {
                var books = await _context.Books.ToListAsync();
                
                if (books == null || !books.Any())
                {
                    _logger.LogWarning("Nenhum livro encontrado na base de dados");
                    return NotFound("No books found.");
                }
                
                _logger.LogInformation("Busca de livros concluída com sucesso. Total encontrado: {BookCount} {@Books}", 
                    books.Count, books);
                
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar livros");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // GET api/<BookController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            _logger.LogInformation("Iniciando busca do livro com ID: {BookId}", id);
            
            try
            {
                var book = await _context.Books.FindAsync(id);
                
                if (book == null)
                {
                    _logger.LogWarning("Livro não encontrado para ID: {BookId}", id);
                    return NotFound($"Book with ID {id} not found.");
                }
                
                _logger.LogInformation("Livro encontrado com sucesso: {@Book}", book);
                return Ok(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar livro com ID: {BookId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // POST api/<BookController>
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book book)
        {
            _logger.LogInformation("Iniciando criação de novo livro: {@BookRequest}", book);
            
            try
            {
                if (book == null)
                {
                    _logger.LogWarning("Tentativa de criar livro com dados nulos");
                    return BadRequest("Book cannot be null.");
                }

                // Validações adicionais se necessário
                if (string.IsNullOrWhiteSpace(book.Title))
                {
                    _logger.LogWarning("Tentativa de criar livro sem título: {@Book}", book);
                    return BadRequest("Book title is required.");
                }
                
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Livro criado com sucesso: {@CreatedBook}", book);
                
                return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar livro: {@Book}", book);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // PUT api/<BookController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(int id, [FromBody] Book updatedBook)
        {
            _logger.LogInformation("Iniciando atualização do livro ID: {BookId} com dados: {@UpdatedBook}", 
                id, updatedBook);
            
            try
            {
                var originalBook = await _context.Books.FindAsync(id);
                
                if (originalBook == null)
                {
                    _logger.LogWarning("Tentativa de atualizar livro inexistente. ID: {BookId}", id);
                    return NotFound($"Book with ID {id} not found.");
                }

                if (updatedBook == null)
                {
                    _logger.LogWarning("Dados de atualização são nulos para livro ID: {BookId}", id);
                    return BadRequest("Updated book data cannot be null.");
                }

                // Criar uma cópia do estado original para auditoria
                var originalState = new Book
                {
                    Id = originalBook.Id,
                    Title = originalBook.Title,
                    Author = originalBook.Author,
                    Genre = originalBook.Genre
                };
                
                // Atualizar os campos
                originalBook.Title = updatedBook.Title;
                originalBook.Author = updatedBook.Author;
                originalBook.Genre = updatedBook.Genre;
                
                await _context.SaveChangesAsync();

                _logger.LogInformation("Livro atualizado com sucesso. ID: {BookId}, Estado original: {@OriginalBook}, Estado atualizado: {@UpdatedBook}", 
                    id, originalState, originalBook);
                
                return Ok(originalBook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar livro ID: {BookId} com dados: {@UpdatedBook}", 
                    id, updatedBook);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        // DELETE api/<BookController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Iniciando remoção do livro ID: {BookId}", id);
            
            try
            {
                var book = await _context.Books.FindAsync(id);
                
                if (book == null)
                {
                    _logger.LogWarning("Tentativa de remover livro inexistente. ID: {BookId}", id);
                    return NotFound($"Book with ID {id} not found.");
                }
                
                // Log do objeto que será removido ANTES da remoção
                _logger.LogWarning("Removendo livro: {@DeletedBook}", book);
                
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Livro removido com sucesso. ID: {BookId}", id);
                
                return Ok(new { message = "Book deleted successfully.", deletedBook = book });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover livro ID: {BookId}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}