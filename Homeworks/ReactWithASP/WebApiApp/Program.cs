using WebApiApp.Extensions;
using WebApiApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(ApiCorsPolicies.AllowSpecificOrigins);

{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
