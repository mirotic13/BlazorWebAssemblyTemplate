# Blazor WebAssembly Template

Este repositorio es una plantilla para aplicaciones **Blazor WebAssembly** con un backend en **ASP.NET Core 8**. Incluye integración con **SQLite** y utiliza **Dapper** como ORM.

## Estructura del Proyecto

- **Client**: Aplicación Blazor WebAssembly.
- **Server**: API REST que gestiona las operaciones de la base de datos.
- **Shared**: Modelos compartidos entre el cliente y el servidor.

## Instrucciones

1. Clona el repositorio y restaura dependencias.
2. Ejecuta el servidor con `dotnet run --project Server`.
3. Accede a la aplicación en `https://localhost:7268`.
4. Para instalar la app como aplicación de escritorio, ejecuta: 
   ```bash
   dotnet publish -c Release -o out
# Luego, utiliza herramientas como Squirrel para empaquetar e instalar
# Más información en: [Squirrel](https://github.com/Squirrel/Squirrel.Windows)
