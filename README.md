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

## Pruebas de rendimiento y capturas en Postman: 
<img width="1232" height="505" alt="image" src="https://github.com/user-attachments/assets/aafef8ac-4231-432b-945f-2d4329c7ade0" />
<img width="1169" height="465" alt="image" src="https://github.com/user-attachments/assets/f44cc12b-d568-49e9-a3e4-8d18d452cd2f" />

Primero se registra -> se inicia sesion -> se hacen las consultas copiando el token generado y agregandolo:
<img width="672" height="254" alt="image" src="https://github.com/user-attachments/assets/9f97422d-f7c0-46bf-9ca5-2df370fa2c16" />

Al hacer el login, el admin es el único que puede sincronizar los films de la api de star wars, SWAPI:
<img width="262" height="64" alt="image" src="https://github.com/user-attachments/assets/955bd7ed-0d4b-4e05-bfb2-98dab0c15547" />
<img width="1162" height="337" alt="image" src="https://github.com/user-attachments/assets/3b2f7d74-cce0-4ae1-ba19-13e545cf6908" />

Al realizarlo, ya se pueden visualizar la lista del metodo get:
<img width="1170" height="878" alt="image" src="https://github.com/user-attachments/assets/f5790000-4f54-4006-9aeb-baf38510f27a" />

Luego los users son los que pueden consultar un film especifico:
<img width="1177" height="381" alt="image" src="https://github.com/user-attachments/assets/647351e6-7f6d-4f98-bc1a-b4bda642aa45" />

Y para finalizar, el admin nuevamente es el único que puede agregar. modificar o eliminar:
<img width="1169" height="593" alt="image" src="https://github.com/user-attachments/assets/26ee4a72-2aee-403c-a872-ab34dd044eb0" />
<img width="1170" height="586" alt="image" src="https://github.com/user-attachments/assets/f6b78312-c12c-4373-839c-9b7317ca64d9" />
<img width="1191" height="211" alt="image" src="https://github.com/user-attachments/assets/40ba03f7-6b1d-42e4-8223-10c8604616f9" />










