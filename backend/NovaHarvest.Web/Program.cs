
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Servir CSS e imágenes desde wwwroot
app.UseStaticFiles();

// Función para generar la estructura común de las páginas
string CrearPagina(string titulo, string contenido)
{
    int año = DateTime.Now.Year;

    return $"""
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>NovaHarvest - {titulo}</title>
        <link rel="stylesheet" href="/css/site.css">
    </head>
    <body>
        <header>
            <img src="/img/logo.jpg" alt="Logo NovaHarvest">
            <h2>NovaHarvest</h2>

            <nav>
                <a href="/">Inicio</a>
                <a href="/about">Acerca de</a>
                <a href="/status">Estado</a>
            </nav>
        </header>

        <main>
            <h1>{titulo}</h1>
            {contenido}
        </main>

        <footer>
            NovaHarvest · IES Valle del Jerte · {año}
        </footer>
    </body>
    </html>
    """;
}

// Página de inicio
app.MapGet("/", () =>
{
    string contenido = """
        <p>NovaHarvest es un proyecto que busca aprovechar los excedentes agrícolas del Valle del Jerte.</p>
        <p>Queremos evitar que alimentos en buen estado se desperdicien.</p>
        <p>La plataforma facilitará la conexión entre productores y personas interesadas en estos productos.</p>
        <p>Así fomentamos un consumo más responsable y sostenible.</p>
        """;

    return Results.Content(CrearPagina("Inicio", contenido), "text/html");
});

// Página Acerca de
app.MapGet("/about", () =>
{
    string contenido = """
        <p><strong>Proyecto:</strong> NovaHarvest</p>
        <p><strong>Centro:</strong> IES Valle del Jerte</p>
        <p><strong>Autor:</strong> Christian Lucia Pulido</p>
        <p><strong>Versión:</strong> 0.1.0</p>
        """;

    return Results.Content(CrearPagina("Acerca de", contenido), "text/html");
});

// Página Estado
app.MapGet("/status", () =>
{
    string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    string contenido = $"""
        <p><strong>Fecha y hora del servidor:</strong> {fecha}</p>
        <p>Página generada por ASP.NET Core (Kestrel)</p>
        """;

    return Results.Content(CrearPagina("Estado del servidor", contenido), "text/html");
});

app.Run();
