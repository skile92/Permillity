using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PermillityLib;
using PermillityTest.Persistence;
using PermillityTest.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IWeatherService, WeatherService>();

var conString = "Server=localhost\\SQLEXPRESS;Database=Permillity;Trusted_Connection=True;TrustServerCertificate=True;";
conString = "Server=127.0.0.1;Port=3306;User ID=root;Pwd=admin;Database=Test;";

builder.Services.AddDbContext<MyDbContext>(x => x.UseSqlServer(conString));
builder.Services.AddLogging();

builder.Services.AddPermillity(x =>
{
    x.UseInMemory();
    x.SetBatchSize(0);
});

var app = builder.Build();

//minimal api
app.MapGet("/todoitems", () =>
{
    var items = new List<string>() { "feed a cat", "buy food"};

    return Results.Ok(items);
});

app.AddPermillityMiddleware();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<MyDbContext>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
