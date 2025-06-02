using eCommerceUsers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
}); 

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

builder.Services.AddSwaggerGen();

// Configure Kestrel for container environments - IMPORTANT for Docker connectivity
builder.WebHost.ConfigureKestrel(options =>
{
    // Force IPv4 binding for better Docker compatibility
    // This ensures we get "http://0.0.0.0:8089" not "http://[::]:8089"
    options.ListenAnyIP(8089, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();

    // DO NOT use HTTPS redirection in development containers
    // app.UseHttpsRedirection(); // <-- Comment this out or remove it
//}
//else
//{
//    app.UseHttpsRedirection();
//}



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.UseAuthorization();

app.MapUserEndpoints();

app.Run();
