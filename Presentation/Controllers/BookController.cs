using Application.Contracts;
using Domain.DTO;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BookController(IBookService bookService, IHttpContextAccessor httpContextAccessor)
    {
        _bookService = bookService;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new Exception("User not authenticated");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(Guid id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] CreateBookDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        await _bookService.AddBookAsync(dto, userId);
        return Ok(new { message = "Book added successfully." });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBook([FromBody] UpdateBookDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _bookService.UpdateBookAsync(dto);
        return Ok(new { message = "Book updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        await _bookService.DeleteBookAsync(id);
        return Ok(new { message = "Book deleted successfully." });
    }

    [HttpGet("my-books")]
    public async Task<IActionResult> GetMyBooks()
    {
        var userId = GetUserId();
        var books = await _bookService.GetBooksByUserAsync(userId);
        return Ok(books);
    }

    [HttpPost("{bookId}/add")]
    public async Task<IActionResult> AddBookToUser(Guid bookId)
    {
        var userId = GetUserId();
        await _bookService.AddBookToUserAsync(bookId, userId);
        return Ok(new { message = "Book added to user successfully." });
    }

    [HttpDelete("{bookId}/remove")]
    public async Task<IActionResult> RemoveBookFromUser(Guid bookId)
    {
        var userId = GetUserId();
        await _bookService.RemoveBookFromUserAsync(bookId, userId);
        return Ok(new { message = "Book removed from user successfully." });
    }
}
