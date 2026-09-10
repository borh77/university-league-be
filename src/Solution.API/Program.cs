using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.HttpOverrides;
using Solution.API.Middleware;
using Solution.API.Startup;
using Solution.Identity.Infrastructure.Database;
using Solution.UniLeague.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);
const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);

builder.Services.RegisterModules();
builder.Services.AddJwtAuthentication();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<IdentityContext>();

        // EnsureCreated ne pravi tabele drugog konteksta ako baza vec postoji,
        // pa se 'identity' sema po potrebi dopravlja rucno - isti obrazac kao u test factory-ju
        if (context.Database.EnsureCreated())
        {
            Console.WriteLine("IDENTITY TABELE SU USPEŠNO KREIRANE IZ KODA!");
        }
        else
        {
            var creator = context.Database.GetService<IRelationalDatabaseCreator>();
            try
            {
                creator.CreateTables();
                Console.WriteLine("IDENTITY TABELE SU DOPRAVLJENE U POSTOJEĆU BAZU.");
            }
            catch
            {
                Console.WriteLine("Identity tabele već postoje.");
            }
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

app.UseStaticFiles();
app.UseRouting();

app.UseCors(corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Required for automated tests
namespace Solution.API
{
    public partial class Program { }
}