# novaharvest-web
# # NovaHarvest

NovaHarvest es una aplicación web destinada al aprovechamiento de excedentes agrícolas del Valle del Jerte. El proyecto utiliza nginx como proxy inverso y ASP.NET Core como backend.

## 1. Arranque

### Ejecutar con Docker

Para construir las imágenes y arrancar los servicios, ejecutar desde la carpeta raíz del proyecto:

docker compose up --build

La aplicación estará disponible en:

- Inicio: http://localhost:8080/
- Acerca de: http://localhost:8080/about
- Estado: http://localhost:8080/status

Para detener los servicios:

docker compose down

### Ejecutar con dotnet run

Para ejecutar el backend directamente, sin Docker ni nginx:

cd backend/NovaHarvest.Web
dotnet run

La dirección y el puerto aparecerán en la terminal. Si se quiere utilizar un puerto concreto:

dotnet run --urls http://localhost:5000

En ese caso, las páginas estarán disponibles en:

- http://localhost:5000/
- http://localhost:5000/about
- http://localhost:5000/status

### Recorrido de una petición

1. El usuario escribe una dirección, por ejemplo `http://localhost:8080/status`.
2. El navegador envía una petición HTTP a nginx.
3. nginx recibe la petición por el puerto 80 del contenedor web.
4. nginx reenvía la petición al servicio backend por el puerto 8080.
5. ASP.NET Core recibe la petición mediante Kestrel y ejecuta el código C# correspondiente.
6. El backend devuelve el HTML generado y nginx lo envía al navegador.

El único servicio que publica un puerto al exterior es nginx. El backend solo es accesible desde la red interna de Docker.

Los archivos CSS e imágenes se encuentran en `wwwroot` y los sirve directamente la aplicación ASP.NET Core.

# Imagen IA

            NAVEGADOR
                 |
                 | HTTP
                 | localhost:8080
                 v
          +-------------+
          |     WEB     |
          |    nginx    |
          | Puerto 80   |
          +-------------+
                 |
                 | Reverse proxy
                 | backend:8080
                 v
          +-------------+
          |   BACKEND   |
          | ASP.NET Core|
          |   Kestrel   |
          | Puerto 8080 |
          +-------------+
                 |
                 v
          Genera las páginas
          y sirve CSS e imágenes

# Cuestiones UT2 – NovaHarvest

## 1. Cliente y servidor

Cuando abro `/status`, el cliente es el navegador, que envía una petición HTTP al servidor.  
El navegador recibe la respuesta y muestra el contenido HTML.  
En el servidor se ejecuta ASP.NET Core mediante Kestrel, que genera la página con código C#.  
También se utiliza nginx para recibir la petición y reenviarla al backend.  
Cuando se puedan reservar productos, la cantidad de kilos disponibles debe comprobarse obligatoriamente en el servidor.  
Esto es necesario porque un usuario podría modificar los datos desde el navegador.  
El servidor debe comprobar siempre que la reserva no supere las existencias reales.

## 2. Estático o dinámico

La página `/about` podría ser un HTML estático porque muestra información que normalmente no cambia, como el nombre del proyecto, el centro y la versión.  
Sin embargo, en NovaHarvest las tres páginas se generan mediante código C# en el servidor, tal como pide el ejercicio.  
La página `/status` no debería ser estática porque muestra la fecha y hora del servidor, que cambian continuamente.  
La página `/` también podría contener información dinámica si se actualizan los productos disponibles.  
Dos ejemplos de páginas que NovaHarvest necesitará y que no podrían ser estáticas son una página de reservas y una página de gestión de existencias.  
Ambas necesitan consultar datos actualizados y procesar las acciones de los usuarios.

## 3. Ejecución en el servidor

Cuando escribo `http://localhost:8080/status`, el navegador envía una petición HTTP a nginx.  
nginx recibe la petición y la reenvía al servicio `backend` por el puerto 8080.  
La aplicación ASP.NET Core recibe la petición mediante Kestrel.  
El método `MapGet("/status", ...)` reconoce la ruta solicitada y ejecuta el código C#.  
El programa obtiene la fecha y hora actuales y genera el HTML de la página.  
Después, la respuesta vuelve a nginx y finalmente llega al navegador.  
Con CGI, se ejecutaría un programa externo para procesar cada petición, en lugar de utilizar directamente el servidor Kestrel.  
Con un módulo embebido, el código se ejecutaría dentro del propio servidor web mediante un módulo integrado.

## 4. Reverse proxy

En NovaHarvest, nginx funciona como reverse proxy porque recibe las peticiones del navegador y las reenvía a ASP.NET Core.  
Para comprobar las cabeceras utilizo `curl -I http://localhost:8080/status` con Docker.  
Después ejecuto el backend directamente con `dotnet run` y repito la petición en el puerto que indique la terminal.  
Con Docker, normalmente aparece `Server: nginx/1.27.x`, mientras que directamente con ASP.NET Core aparece `Server: Kestrel`.  
Esto ocurre porque con Docker la respuesta pasa por nginx y sin nginx responde directamente Kestrel.  
Al ejecutar `docker compose stop backend`, las peticiones `/status` y `/css/site.css` deberían devolver un error HTTP 502 porque nginx no puede contactar con el backend.  
El backend no publica ningún puerto porque solo necesita recibir peticiones desde nginx dentro de la red de Docker.

**Resultados reales de mis comprobaciones:**

- Docker: `Server: [ESCRIBE AQUÍ EL RESULTADO REAL]`
- dotnet run: `Server: [ESCRIBE AQUÍ EL RESULTADO REAL]`
- `/status` con backend detenido: `[CÓDIGO HTTP]`
- `/css/site.css` con backend detenido: `[CÓDIGO HTTP]`

## 5. nginx

nginx no sirve directamente `site.css` ni `logo.png` porque está configurado para reenviar todas las peticiones al backend.  
Es ASP.NET Core quien sirve los archivos estáticos que están dentro de la carpeta `wwwroot`.  
De esta manera, la aplicación se encarga tanto de generar las páginas como de proporcionar sus estilos e imágenes.  
nginx se utiliza como intermediario entre el navegador y el backend.  
Si NovaHarvest se publicara en Internet, nginx podría aportar dos funciones importantes.  
La primera sería gestionar conexiones HTTPS mediante certificados SSL/TLS.  
La segunda sería limitar peticiones o distribuir el tráfico entre varios servidores backend.

## 6. Tecnologías

Si la cooperativa quisiera utilizar otra tecnología de servidor, habría que cambiar principalmente el backend.  
Por ejemplo, si se utilizara PHP, habría que sustituir el proyecto ASP.NET Core y sus archivos C#.  
También cambiarían el Dockerfile del backend y la imagen Docker utilizada para ejecutar esa tecnología.  
Los archivos de nginx y su configuración podrían mantenerse porque seguirían realizando la función de proxy inverso.  
El archivo `compose.yaml` también podría mantenerse, aunque habría que adaptar la construcción del backend si cambia su estructura.  
El navegador seguiría accediendo a la misma dirección y las rutas podrían conservarse.  
Los estilos CSS y las imágenes también podrían mantenerse si se respetan sus direcciones.

## 7. Marcado y código

Un ejemplo de generación de HTML en mi `Program.cs` es el siguiente:

app.MapGet("/status", () =>
{
    string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    string contenido = $"""
        <p><strong>Fecha y hora del servidor:</strong> {fecha}</p>
        <p>Página generada por ASP.NET Core (Kestrel)</p>
        """;

    return Results.Content(CrearPagina("Estado del servidor", contenido), "text/html");
});

Generar HTML mediante cadenas tiene algunos inconvenientes.  
El primero es que resulta más difícil mantener el código cuando las páginas tienen mucho contenido.  
El segundo es que es más fácil cometer errores al escribir etiquetas HTML o al introducir datos dentro de las cadenas.  
Además, si se repite mucho código, los cambios deben hacerse en varios lugares.  
En mi proyecto se repite la estructura común de HTML, el encabezado, el menú de navegación, los enlaces a CSS y el pie de página.  
Para evitar repetirla, he creado la función `CrearPagina()`, que genera la estructura común de las tres páginas.

## 8. Docker

En mi proyecto hay dos imágenes principales de la aplicación: una para nginx y otra para ASP.NET Core.  
También hay dos contenedores cuando el entorno está levantado: `web` y `backend`.  
Una imagen es una plantilla que contiene lo necesario para crear un contenedor.  
Un contenedor es una instancia de esa imagen que se está ejecutando.  
La imagen final del backend no incluye el SDK porque solo necesita el entorno de ejecución de ASP.NET Core para arrancar la aplicación publicada.  
El SDK se utiliza en la primera etapa del Dockerfile para compilar el proyecto.  
Sin el archivo `.dockerignore`, Docker podría copiar archivos innecesarios como `bin`, `obj` o `.git`, aumentando el contexto de construcción y pudiendo introducir archivos que no hacen falta.

## 9. Herramientas

| Herramienta  --> Problema que resuelve en NovaHarvest 

| Visual Studio Code --> Escribir y editar el código del proyecto.
| C# --> Generar las páginas desde el servidor.
| ASP.NET Core --> Crear el backend y gestionar las rutas.
| Kestrel --> Recibir y responder a las peticiones HTTP.
| nginx --> Actuar como reverse proxy.
| Docker --> Crear y ejecutar los contenedores.
| Docker Compose --> Gestionar conjuntamente los servicios web y backend.
| HTML --> Crear la estructura de las páginas.
| CSS --> Dar estilo y diseño a la web. 
| Git y GitHub --> Guardar versiones del proyecto y compartir el código. 
| Navegador web --> Acceder a la aplicación y comprobar su funcionamiento. 
| curl --> Realizar peticiones HTTP y consultar las cabeceras. 

## 10. Observación

La página `/status` muestra la fecha y hora obtenidas mediante `DateTime.Now`.  
Cuando ejecuto el proyecto con `dotnet run`, utiliza la hora y zona horaria del sistema donde se está ejecutando.  
Cuando lo ejecuto con Docker, utiliza la configuración horaria del contenedor.  
Por eso, las horas podrían no coincidir si el contenedor utiliza UTC y el ordenador utiliza la hora local de España.  
En mi caso, tengo que comparar las dos horas para comprobar si existe alguna diferencia.  
Si no coinciden, no significa que la aplicación esté funcionando mal, sino que los entornos pueden tener zonas horarias diferentes.  
Para conseguir que coincidan, se puede configurar la zona horaria del contenedor.

