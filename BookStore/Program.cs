using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastucture.Extensions;
using Domain.Contracts;
using Infrastucture.Repositories;
using Application.Contracts;
using Application.Services;
using Microsoft.OpenApi.Models;
using Infrastructure.Seed;


public class Program
{
    public static async Task Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);

        // ? Add DbContext
        builder.Services.AddDbContext<BookStoreContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // ? Add IdentityCore (if you're not using full Identity UI)
        builder.Services.AddIdentityCore<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<BookStoreContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.ConfigureJwt(builder.Configuration);
        builder.Services.ConfigureCors();
        builder.Services.ConfigureServices();
        builder.Services.ConfigureOpenApi(); // Swagger configuration
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<IBookService, BookService>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        var app = builder.Build();

        // Seed admin user before running the app
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<BookStoreContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            await SeedData.InitializeAsync(context, userManager);
            context.Database.Migrate();

            var user = await userManager.FindByEmailAsync("admin.admin@admin.com");
            if (user == null)
            {
                user = new User
                {
                    UserName = "admin",
                    Email = "admin.admin@admin.com"
                };

                await userManager.CreateAsync(user, "password");
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
