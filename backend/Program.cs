var builder = WebApplication.CreateBuilder(args);

// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

// Usar CORS
app.UseCors("AllowLocalhost3000");

app.UseAuthorization();

app.MapControllers();

app.Run();