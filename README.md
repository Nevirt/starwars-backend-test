# StarWars Backend (.NET 8 Web API)

Backend para gestionar películas usando datos de SWAPI y autenticación JWT.

## Requisitos
- .NET SDK 8.0+

## Ejecutar localmente
```bash
# Restaurar y compilar
dotnet build

# Crear DB y aplicar migraciones
cd StarWars.Api
# (opcional si no existe) dotnet tool install --global dotnet-ef
# dotnet ef migrations add InitialCreate
# dotnet ef database update

# Ejecutar API
dotnet run
```

Por defecto se inicia en `http://localhost:5242` y `https://localhost:7010` con Swagger en `/swagger`.

## Configuración
- `appsettings.json` define `ConnectionStrings:DefaultConnection`, `Jwt` y `Swapi:BaseUrl`.
- Clave JWT de desarrollo está incluida. Cambiar para producción.

## Autenticación y Roles
- SignUp/Login devuelven JWT.
- Roles: `User`, `Admin`.
- Accesos:
  - GET `/api/movies`: público
  - GET `/api/movies/{id}`: requiere `User` (no Admin)
  - POST/PUT/DELETE `/api/movies`: requiere `Admin`
  - POST `/api/admin/sync-films`: requiere `Admin`

## Endpoints principales
- POST `/api/auth/signup` { email, password, role }
- POST `/api/auth/login` { email, password }
- GET `/api/movies`
- GET `/api/movies/{id}`
- POST `/api/movies`
- PUT `/api/movies/{id}`
- DELETE `/api/movies/{id}`
- POST `/api/admin/sync-films`

## Swagger
- Disponible en `/swagger`.
- Agregar `Authorize` con esquema Bearer para probar endpoints protegidos.

## Tests
```bash
dotnet test
```
Incluye pruebas de integración para auth, movies y autorización.
