using Microsoft.EntityFrameworkCore;
using Models;
using Services.Intefaces;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISmartWatchService, SmartWatchService>();
builder.Services.AddDbContext<WatchContext>(opt =>
{
    opt.UseNpgsql("user id=postgres;password=password;host=localhost;database=postgres");
});
var app = builder.Build();

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
