using Domain.Contracts;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastucture.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookStoreContext _context;
        public BookRepository(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync() =>
            await _context.Books.ToListAsync();

        public async Task<Book?> GetByIdAsync(Guid id) =>
            await _context.Books.FindAsync(id);

        public async Task AddAsync(Book book) =>
            await _context.Books.AddAsync(book);

        public async Task UpdateAsync(Book book) =>
            _context.Books.Update(book);

        public async Task DeleteAsync(Book book) =>
            _context.Books.Remove(book);

        public async Task SaveAsync() =>
            await _context.SaveChangesAsync();
    }

}
