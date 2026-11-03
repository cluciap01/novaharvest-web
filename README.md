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