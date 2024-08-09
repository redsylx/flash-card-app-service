using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Main;
using Main.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.SetAuthentication();
builder.SetCors();
builder.SupressInvalidFilter();
builder.Services.AddDbContext<Context>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();

app.Run();