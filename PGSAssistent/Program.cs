using PGSAssistent.Configuration;
using PGSAssistent.Services;
using PGSAssistentAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Gemini")
    );

builder.Services.AddHttpClient<FileSearchService>(http =>
{
    http.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    http.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<ConversationalService>();
var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();


app.Run();
