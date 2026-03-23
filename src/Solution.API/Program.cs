using Microsoft.EntityFrameworkCore;
using Solution.API.Middleware;
using Solution.API.Startup;
// Dodaj using za tvoj DB context - proveri da li je putanja tačna
using Solution.UniLeague.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);
const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);

builder.Services.RegisterModules();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UniLeagueContext>();

        if (context.Database.EnsureCreated())
        {
            Console.WriteLine("TABELE SU USPEŠNO KREIRANE IZ KODA!");
        }
        else
        {
            Console.WriteLine("Tabele već postoje ili su migracije već odrađene.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"GREŠKA: {ex.Message}");
    }
}
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

// Required for automated tests
namespace Solution.API
{
    public partial class Program { }
}