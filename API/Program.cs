using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, ProductRepository>(); 

var app = builder.Build();

app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    StoreContextSeed.SeedAsync(context).Wait();
    Console.WriteLine("Database seeded successfully.");
}
catch (Exception ex)
{
    // Log the exception (not implemented here for brevity)
    Console.WriteLine($"An error occurred during database seeding: {ex.Message}");
    throw;
}

app.Run(); 
