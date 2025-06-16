using Bogus;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Contexts;

namespace Infrastructure.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            BookStoreContext context,
            UserManager<User> userManager)
        {

            await context.Database.MigrateAsync();


            if (!context.Books.Any())
            {
                var bookFaker = new Faker<Book>()
                    .RuleFor(b => b.Title, f => f.Lorem.Sentence(3, 5))
                    .RuleFor(b => b.Author, f => f.Name.FullName())
                    .RuleFor(b => b.PublishedDate, f => f.Date.Past(30));

                var books = bookFaker.Generate(100);
                context.Books.AddRange(books);

                await context.SaveChangesAsync();
            }


            if (!userManager.Users.Any())
            {
                var userFaker = new Faker<User>()
                    .RuleFor(u => u.UserName, f => f.Internet.UserName())
                    .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.UserName))
                    .RuleFor(u => u.EmailConfirmed, true);

                var password = "Password!123";

                for (int i = 0; i < 5; i++)
                {
                    var user = userFaker.Generate();
                    var existingUser = await userManager.FindByEmailAsync(user.Email);
                    if (existingUser == null)
                    {
                        var result = await userManager.CreateAsync(user, password);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                        }
                    }
                }
            }
        }
    }
}
