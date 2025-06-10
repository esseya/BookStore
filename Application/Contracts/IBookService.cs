using Domain.DTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto?> GetBookByIdAsync(Guid id);
        Task AddBookAsync(CreateBookDto dto, string userId);
        Task UpdateBookAsync(UpdateBookDto dto);
        Task DeleteBookAsync(Guid id);
        Task<IEnumerable<BookDto>> GetBooksByUserAsync(string userId);
        Task AddBookToUserAsync(Guid bookId, string userId);
        Task RemoveBookFromUserAsync(Guid bookId, string userId);
    }


}
