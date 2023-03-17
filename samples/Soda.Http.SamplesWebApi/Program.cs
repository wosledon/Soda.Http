using System.Net;
using Soda.Http;
using Soda.Http.Core;
using Soda.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSodaHttp(opts =>
{
    opts.BaseUrl = "http://localhost:8080/";
    opts.Accept = new[]
    {
        "application/json",
        "text/plain",
        "*/*"
    };
    opts.EnableCompress = false;
    opts.Headers = new[]{
        ("X-Ca-Test", "key")
    };
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
