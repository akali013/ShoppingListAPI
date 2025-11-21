using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingListAPI.Authorization;
using ShoppingListAPI.Data;
using ShoppingListAPI.Helpers;
using ShoppingListAPI.Services;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
var AllowSpecificOrigins = "_AllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSpecificOrigins,
            policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            }
        );
});

// Add services to the container.
// JWT system created by: https://jasonwatmore.com/net-6-jwt-authentication-with-refresh-tokens-tutorial-with-example-api
builder.Services.AddDbContext<ShoppingListAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShoppingListAPIContext") ?? throw new InvalidOperationException("Connection string 'ShoppingListAPIContext' not found.")));
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IShoppingListItemService, ShoppingListItemService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors(AllowSpecificOrigins);

// Global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();
// For inserting the user object in requests with valid JWTs
app.UseMiddleware<JwtMiddleware>();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine(Guid.NewGuid());
Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("password123"));
Console.WriteLine(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)));

app.Run();