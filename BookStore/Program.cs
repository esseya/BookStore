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


        builder.Services.AddDbContext<BookStoreContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.EnableRetryOnFailure()
            ));

        builder.Services.AddIdentityCore<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<BookStoreContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowStaticWebApp",
                policy =>
                {
                    policy.WithOrigins("https://agreeable-island-0bd510f03.6.azurestaticapps.net")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        builder.Services.AddControllers();
        builder.Services.ConfigureJwt(builder.Configuration);
        //builder.Services.ConfigureCors();
        builder.Services.ConfigureServices();
        //builder.Services.ConfigureOpenApi(); 
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });

        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<IBookService, BookService>();

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        var app = builder.Build();

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

        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowStaticWebApp");
        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                Console.WriteLine("CORS headers:");
                foreach (var header in context.Response.Headers)
                {
                    if (header.Key.StartsWith("Access-Control"))
                        Console.WriteLine($"{header.Key}: {header.Value}");
                }
                return Task.CompletedTask;
            });

            await next();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
