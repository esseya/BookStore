using Application.Contracts;
using AutoMapper;
using Domain.Contracts;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepo;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public BookService(IBookRepository bookRepo, UserManager<User> userManager, IMapper mapper)
    {
        _bookRepo = bookRepo;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _bookRepo.GetAllAsync();
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepo.GetByIdAsync(id);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task AddBookAsync(CreateBookDto dto, string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Books)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found.");

        var book = _mapper.Map<Book>(dto);
        book.Id = Guid.NewGuid(); // You handle GUID manually here

        user.Books.Add(book);
        await _bookRepo.AddAsync(book);
        await _bookRepo.SaveAsync();
    }

    public async Task UpdateBookAsync(UpdateBookDto dto)
    {
        var existingBook = await _bookRepo.GetByIdAsync(dto.Id);
        if (existingBook == null)
            throw new Exception("Book not found.");

        _mapper.Map(dto, existingBook); // Updates properties
        await _bookRepo.UpdateAsync(existingBook);
        await _bookRepo.SaveAsync();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var book = await _bookRepo.GetByIdAsync(id);
        if (book != null)
        {
            await _bookRepo.DeleteAsync(book);
            await _bookRepo.SaveAsync();
        }
    }

    public async Task<IEnumerable<BookDto>> GetBooksByUserAsync(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Books)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return Enumerable.Empty<BookDto>();

        return _mapper.Map<IEnumerable<BookDto>>(user.Books);
    }

    public async Task AddBookToUserAsync(Guid bookId, string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Books)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var book = await _bookRepo.GetByIdAsync(bookId);

        if (user != null && book != null && !user.Books.Contains(book))
        {
            user.Books.Add(book);
            await _bookRepo.SaveAsync();
        }
    }

    public async Task RemoveBookFromUserAsync(Guid bookId, string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.Books)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var book = await _bookRepo.GetByIdAsync(bookId);

        if (user != null && book != null && user.Books.Contains(book))
        {
            user.Books.Remove(book);
            await _bookRepo.SaveAsync();
        }
    }
}


