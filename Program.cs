using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Main;

var builder = WebApplication.CreateBuilder(args);

builder.SetAuthentication();
builder.SetCors();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();