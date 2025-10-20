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

## Seguridad: JWT + ApiKey
- JWT
  - Debes configurar `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` y `Jwt:ExpirationMinutes`.
  - Flujo: `POST /api/auth/signup` o `POST /api/auth/login` devuelve `token` (JWT). Usar en `Authorization: Bearer <token>`.
- ApiKey (capa extra)
  - Header configurable en `ApiKeyConfiguration:Header` (por defecto `ApiKey`).
  - Realm: `ApiKeyConfiguration:Realm`.
  - Key: `ApiKeyConfiguration:Key` (requerido en todas las requests excepto `/swagger` y `/health`).
  - Ejemplo curl:
```bash
curl -H "ApiKey: sUApBf-2QBOra+~_(o*G~gd4JKD0#" -H "Authorization: Bearer <JWT>" http://localhost:5242/api/movies
```
  - En Swagger, habilita ambos: botón Authorize para `Bearer` y campo `ApiKey` (encabezado).

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

Notas de autorización
- Todos los endpoints requieren ApiKey en el header (`ApiKey: ...`).
- Adicionalmente:
  - `GET /api/movies`: anónimo (solo ApiKey).
  - `GET /api/movies/{id}`: requiere JWT con rol `User`.
  - `POST|PUT|DELETE /api/movies` y `POST /api/admin/sync-films`: requieren JWT con rol `Admin`.

## Swagger
- Disponible en `/swagger`.
- Agregar `Authorize` con esquema Bearer para probar endpoints protegidos.

## Tests
```bash
dotnet test
```
Incluye pruebas de integración para auth, movies y autorización.

### Cobertura (Code Coverage)
```bash
# Ejecutar pruebas con cobertura (coverlet collector)
dotnet test --collect:'XPlat Code Coverage'

# Generar reporte HTML (una vez)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:StarWars.Tests/TestResults/*/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html

# Abrir el reporte
# abre el archivo coveragereport/index.html en el navegador
```
Los tests incluyen caminos de éxito y error: ApiKey faltante/incorrecta (401), rol incorrecto (403), 404 para recursos inexistentes, validaciones de signup (400) y login inválido (401).

## Postman
- Importar colección y environment desde la carpeta `postman/`:
  - `postman/StarWars.postman_collection.json`
  - `postman/StarWars.postman_environment.json`
- Variables usadas: `baseUrl`, `apiKey`, `jwt`, `movieId`.
- Flujo sugerido:
  1) Ejecuta SignUp o Login para obtener `jwt` y colócalo en el environment.
  2) Probar endpoints de Movies (añadir `movieId` devuelto por Create).
  3) Probar `Admin/Sync Films` con un JWT de Admin.
