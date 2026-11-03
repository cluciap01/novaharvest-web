var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/", () =>
{
    return Results.Content("""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <title>NovaHarvest</title>
            <link rel="stylesheet" href="/css/site.css">
        </head>
        <body>
            <header>
                <img src="/img/logo.png" alt="Logo NovaHarvest">
                <h1>NovaHarvest</h1>
            </header>

            <main>
                <h2>Página servida por ASP.NET Core</h2>
                <p>Próximamente: aprovechamiento de excedentes del Valle del Jerte.</p>
            </main>
        </body>
        </html>
        """, "text/html");
});

app.Run();