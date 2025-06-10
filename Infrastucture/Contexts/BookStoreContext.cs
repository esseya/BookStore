using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using Domain.Entities;

namespace Infrastructure.Contexts;

public class BookStoreContext : IdentityDbContext<User, IdentityRole, string>
{
    public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
    {
       
}
    public DbSet<Book> Books { get; set; }
}

