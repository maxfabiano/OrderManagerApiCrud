using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Repository;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Database.Repository;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Anti-Forgery service
builder.Services.AddAntiforgery(options =>
{
    // Optional: Configure Anti-Forgery options here
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ecomerce",
        Description = "Cadastro De Pedidos"
    });
});
builder.Services.AddMediatR(typeof(createPedidoCommand).Assembly);
SQLitePCL.Batteries.Init();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cadastro De Pedidos");
        c.RoutePrefix = string.Empty;
    });
}

// Add Anti-Forgery middleware after UseRouting
app.UseAntiforgery();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();