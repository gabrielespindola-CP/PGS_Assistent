using PGSAssistent.Configuration;
using PGSAssistent.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Gemini")
    );

builder.Services.AddScoped<GeminiService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();


app.Run();
