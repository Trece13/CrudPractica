# CRUD .NET 9 — Guía de preparación y contingencia

Guía rápida para preparar un equipo, restaurar dependencias, trabajar con .NET 9, Entity Framework Core, SQL Server, migrations y scaffolding MVC.

---

# 1. Verificar .NET

Verificar la versión activa:

```powershell
dotnet --version
```

Ver todos los SDK instalados:

```powershell
dotnet --list-sdks
```

Información completa:

```powershell
dotnet --info
```

El proyecto utiliza:

```text
.NET 9
```

Por lo tanto debe existir un SDK `9.0.x`.

Ejemplo:

```text
9.0.xxx
```

> Importante: se necesita el **SDK**, no solamente el Runtime, para desarrollar y compilar.

---

# 2. Instalar .NET 9 sin permisos de administrador

Si el computador no tiene .NET 9 y no se tienen permisos de administrador, se puede instalar dentro del perfil del usuario utilizando `dotnet-install.ps1`.

Si el script ya está disponible:

```powershell
powershell -ExecutionPolicy Bypass -File .\dotnet-install.ps1 -Channel 9.0 -InstallDir "$env:USERPROFILE\.dotnet"
```

Configurar la terminal actual:

```powershell
$env:DOTNET_ROOT="$env:USERPROFILE\.dotnet"
$env:PATH="$env:USERPROFILE\.dotnet;$env:PATH"
```

Verificar:

```powershell
dotnet --version
```

La instalación quedará aproximadamente en:

```text
C:\Users\USUARIO\.dotnet
```

Esto evita depender de una instalación en:

```text
C:\Program Files\dotnet
```

---

# 3. Restaurar un proyecto descargado de Git

Después de clonar o copiar el proyecto:

```powershell
dotnet restore
```

Este comando lee los archivos `.csproj` y descarga automáticamente los paquetes NuGet requeridos.

Después:

```powershell
dotnet build
```

Resultado esperado:

```text
Build succeeded.
```

Secuencia recomendada:

```powershell
dotnet restore
dotnet build
```

---

# 4. Verificar paquetes NuGet

Ver paquetes de Infrastructure:

```powershell
dotnet list CrudPractica.Infrastructure package
```

Ver paquetes de API:

```powershell
dotnet list CrudPractica.Api package
```

Ver paquetes de MVC:

```powershell
dotnet list CrudPractica.MvcApi package
```

Esto permite verificar las versiones realmente instaladas/resueltas.

---

# 5. Entity Framework Core 9

El proyecto utiliza EF Core `9.0.x`.

En este proyecto se ha trabajado con:

```text
Microsoft.EntityFrameworkCore.SqlServer 9.0.20
Microsoft.EntityFrameworkCore.Design    9.0.20
Microsoft.EntityFrameworkCore.Tools     9.0.20
```

> Preferiblemente mantener todos los paquetes de Entity Framework Core en la misma versión.

---

# 6. Instalar paquetes EF Core manualmente

Normalmente esto NO será necesario al clonar el repositorio porque:

```powershell
dotnet restore
```

restaura lo declarado en los `.csproj`.

Si se está creando un proyecto desde cero o falta algún paquete:

## SQL Server Provider

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.20
```

Permite utilizar:

```csharp
options.UseSqlServer(...)
```

## EF Core Design

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.Design --version 9.0.20
```

Necesario para operaciones de diseño como migrations.

## EF Core Tools

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.Tools --version 9.0.20
```

Necesario para tooling de Entity Framework y algunos escenarios de scaffolding.

Después:

```powershell
dotnet restore
dotnet build
```

---

# 7. Verificar EF CLI

Ejecutar:

```powershell
dotnet ef --version
```

Si `dotnet ef` no existe:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.20
```

Si ya existe pero se necesita actualizar:

```powershell
dotnet tool update --global dotnet-ef --version 9.0.20
```

Verificar nuevamente:

```powershell
dotnet ef --version
```

---

# 8. Alternativa: dotnet-ef local al repositorio

Para no depender de la instalación global de `dotnet-ef`, puede utilizarse un Tool Manifest.

Crear el manifest una sola vez:

```powershell
dotnet new tool-manifest
```

Agregar EF:

```powershell
dotnet tool install dotnet-ef --version 9.0.20
```

Esto genera:

```text
.config/
└── dotnet-tools.json
```

Este archivo puede guardarse en Git.

En otro computador solamente se necesita:

```powershell
dotnet tool restore
```

Y se puede comprobar con:

```powershell
dotnet tool run dotnet-ef --version
```

---

# 9. Verificar SQL Server

Comprobar herramientas SQL:

```powershell
sqlcmd -?
```

Comprobar LocalDB:

```powershell
sqllocaldb info
```

Si aparece:

```text
MSSQLLocalDB
```

LocalDB está disponible.

Iniciarlo:

```powershell
sqllocaldb start MSSQLLocalDB
```

Ver información:

```powershell
sqllocaldb info MSSQLLocalDB
```

---

# 10. Connection Strings

## SQL Server + Windows Authentication

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CrudPractica;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## SQL Server Express

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CrudPractica;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## LocalDB

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CrudPractica;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Resumen:

```text
SQL Server  → localhost
SQL Express → .\SQLEXPRESS
LocalDB     → (localdb)\MSSQLLocalDB
```

---

# 11. SQL Server y permisos de administrador

.NET SDK puede instalarse dentro del perfil del usuario sin administrador.

SQL Server completo es diferente porque instala servicios y componentes de Windows.

Plan recomendado:

```text
PLAN A
SQL Server instalado
        ↓
usar localhost

PLAN B
SQL Express / LocalDB disponible
        ↓
usar esa instancia

PLAN C
No existe SQL Server disponible
        ↓
usar SQLite como contingencia
```

---

# 12. Registrar DbContext

Ejemplo en `Program.cs`:

```csharp
builder.Services.AddDbContext<PollitoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

Y el contexto:

```csharp
public class PollitoDbContext : DbContext
{
    public PollitoDbContext(
        DbContextOptions<PollitoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}
```

Conceptualmente:

```text
appsettings.json
      ↓
ConnectionString
      ↓
Program.cs
      ↓
AddDbContext
      ↓
DbContextOptions
      ↓
PollitoDbContext
      ↓
SQL Server
```

---

# 13. MIGRATIONS — PROYECTO NUEVO

Este es el procedimiento cuando se crea el modelo desde cero y todavía no existe la base de datos.

Supongamos:

```text
Domain
    ↓
Product.cs
Category.cs

Infrastructure
    ↓
PollitoDbContext

API
    ↓
Program.cs
```

Primero compilar:

```powershell
dotnet build
```

Después crear la primera migration:

```powershell
dotnet ef migrations add InitialCreate --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Significado:

```text
--project CrudPractica.Infrastructure

¿Dónde está el DbContext y dónde se guardan
las migrations?


--startup-project CrudPractica.Api

¿Qué proyecto arranca y proporciona
Program.cs, DI y configuración?


--context PollitoDbContext

¿Qué DbContext debe utilizar EF?
```

Se generará una carpeta similar a:

```text
CrudPractica.Infrastructure/
└── Migrations/
    ├── XXXXX_InitialCreate.cs
    ├── XXXXX_InitialCreate.Designer.cs
    └── PollitoDbContextModelSnapshot.cs
```

---

# 14. Aplicar la primera migration

Después de crearla:

```powershell
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Esto toma:

```text
Entidades
   ↓
DbContext
   ↓
Migration
   ↓
database update
   ↓
SQL Server
```

y crea/actualiza físicamente la base de datos.

---

# 15. MODIFICAR EL MODELO Y CREAR OTRA MIGRATION

Este será un escenario común durante una prueba técnica.

Ejemplo:

Inicialmente existe:

```text
Product
```

Después se agrega:

```text
Category
```

y una relación:

```text
Product N ←→ N Category
```

Primero modificar las entidades y `DbContext`.

Después:

```powershell
dotnet build
```

Crear una nueva migration:

```powershell
dotnet ef migrations add AddProductCategories --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

EF compara:

```text
ModelSnapshot anterior
        VS
Modelo actual
```

y genera los cambios necesarios.

---

# 16. Aplicar la nueva migration

Después:

```powershell
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Por ejemplo:

```text
ANTES

Products


CAMBIO EN CÓDIGO

Product
   N
   ↕
   N
Category


MIGRATION

AddProductCategories


DATABASE UPDATE

Products
Categories
ProductCategories
```

---

# 17. Flujo que se debe memorizar

Cada vez que se modifica el esquema:

```text
1. Modificar Entity / DbContext
          ↓
2. dotnet build
          ↓
3. migrations add
          ↓
4. revisar migration
          ↓
5. database update
          ↓
6. probar aplicación
```

Comandos:

```powershell
dotnet build

dotnet ef migrations add NombreMigracion --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext

dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

---

# 18. Ver migrations existentes

```powershell
dotnet ef migrations list --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Permite verificar:

```text
InitialCreate
AddProductCategories
...
```

---

# 19. Eliminar la última migration

Si se creó una migration incorrecta y todavía se está trabajando localmente:

```powershell
dotnet ef migrations remove --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Después se corrige el modelo y se vuelve a generar:

```powershell
dotnet ef migrations add NombreCorrecto --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

> Tener cuidado con migrations que ya fueron aplicadas en bases compartidas o producción.

---

# 20. PROYECTO CLONADO CON MIGRATIONS EXISTENTES

Si el repositorio ya contiene:

```text
Infrastructure/
└── Migrations/
```

NO se debe volver a ejecutar:

```text
migrations add InitialCreate
```

Las migrations ya existen.

Solamente:

```powershell
dotnet restore
dotnet build
```

y después:

```powershell
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

EF ejecutará las migrations pendientes.

Flujo:

```text
git clone
   ↓
dotnet restore
   ↓
dotnet build
   ↓
database update
   ↓
BD lista
```

---

# 21. Diferencia importante

## Estoy creando/modificando el modelo

Necesito:

```text
migrations add
       +
database update
```

Ejemplo:

```powershell
dotnet ef migrations add AddCategories --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext

dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

## Cloné un proyecto cuyas migrations ya existen

Necesito solamente:

```text
database update
```

Ejemplo:

```powershell
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

---

# 22. Scaffolding MVC

Para generar Controller + Views mediante Visual Studio se requieren los paquetes correspondientes.

Instalar si es necesario:

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.20
```

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.Design --version 9.0.20
```

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.Tools --version 9.0.20
```

```powershell
dotnet add NOMBRE_PROYECTO package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 9.0.0
```

Después:

```powershell
dotnet restore
dotnet build
```

Desde Visual Studio:

```text
Controllers
   ↓
Add
   ↓
New Scaffolded Item
   ↓
MVC Controller with views,
using Entity Framework
```

Seleccionar:

```text
Model:
Product

DbContext:
AppDbContext

Controller:
ProductsController
```

El scaffolding genera normalmente:

```text
Controllers/
└── ProductsController.cs

Views/
└── Products/
    ├── Index.cshtml
    ├── Create.cshtml
    ├── Edit.cshtml
    ├── Details.cshtml
    └── Delete.cshtml
```

---

# 23. MVC consumiendo Web API

Si las Views scaffoldeadas se van a utilizar con una API separada, el scaffolding puede utilizarse inicialmente para generar las Views.

Después se reemplaza:

```text
MVC Controller
      ↓
DbContext
```

por:

```text
MVC Controller
      ↓
HttpClient
      ↓
Web API
      ↓
Service
      ↓
Repository
      ↓
DbContext
      ↓
SQL Server
```

Ejemplo de registro:

```csharp
builder.Services.AddHttpClient("ProductsApi", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiSettings:BaseUrl"]!);
});
```

---

# 24. Ejecutar API

```powershell
dotnet run --project CrudPractica.Api
```

---

# 25. Ejecutar MVC

```powershell
dotnet run --project CrudPractica.MvcApi
```

Si MVC consume la API, ambos proyectos deben estar ejecutándose.

En Visual Studio:

```text
Solution
   ↓
Configure Startup Projects
   ↓
Multiple startup projects

CrudPractica.Api       → Start
CrudPractica.MvcApi    → Start
```

---

# 26. Checklist rápido para un computador desconocido

Ejecutar en este orden:

```powershell
dotnet --version
dotnet --list-sdks
dotnet --info
```

Después:

```powershell
dotnet restore
dotnet build
```

Verificar SQL:

```powershell
sqlcmd -?
sqllocaldb info
```

Verificar EF:

```powershell
dotnet ef --version
```

Ver paquetes:

```powershell
dotnet list CrudPractica.Infrastructure package
dotnet list CrudPractica.Api package
dotnet list CrudPractica.MvcApi package
```

---

# 27. Checklist para crear una base desde mi código

Si acabo de crear las entidades y el DbContext:

```powershell
dotnet build
```

Crear migration:

```powershell
dotnet ef migrations add InitialCreate --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Aplicar:

```powershell
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

---

# 28. Checklist después de modificar entidades

Por ejemplo, agregué una tabla, columna, FK o relación:

```powershell
dotnet build
```

Después:

```powershell
dotnet ef migrations add NombreDelCambio --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Después:

```powershell
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext
```

Finalmente:

```powershell
dotnet build
```

---

# 29. Resumen de comandos esenciales

```powershell
# ==============================
# VERIFICAR .NET
# ==============================

dotnet --version
dotnet --list-sdks
dotnet --info


# ==============================
# RESTAURAR Y COMPILAR
# ==============================

dotnet restore
dotnet build


# ==============================
# VERIFICAR SQL
# ==============================

sqlcmd -?
sqllocaldb info


# ==============================
# VERIFICAR EF
# ==============================

dotnet ef --version


# ==============================
# INSTALAR EF CLI SI FALTA
# ==============================

dotnet tool install --global dotnet-ef --version 9.0.20


# ==============================
# PAQUETES EF SI FALTAN
# ==============================

dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.20

dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.Design --version 9.0.20

dotnet add NOMBRE_PROYECTO package Microsoft.EntityFrameworkCore.Tools --version 9.0.20


# ==============================
# SCAFFOLDING MVC
# ==============================

dotnet add NOMBRE_PROYECTO package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 9.0.0


# ==============================
# PRIMERA MIGRATION
# ==============================

dotnet ef migrations add InitialCreate --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext


# ==============================
# APLICAR MIGRATIONS
# ==============================

dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext


# ==============================
# NUEVA MIGRATION DESPUÉS
# DE MODIFICAR EL MODELO
# ==============================

dotnet ef migrations add NombreMigracion --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext

dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext


# ==============================
# VER MIGRATIONS
# ==============================

dotnet ef migrations list --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext


# ==============================
# ELIMINAR ÚLTIMA MIGRATION
# ==============================

dotnet ef migrations remove --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext


# ==============================
# EJECUTAR
# ==============================

dotnet run --project CrudPractica.Api

dotnet run --project CrudPractica.MvcApi
```

---

# 30. Flujo mental mínimo

## Proyecto nuevo

```text
Crear entidades
      ↓
Crear DbContext
      ↓
Registrar DbContext en DI
      ↓
Configurar ConnectionString
      ↓
dotnet build
      ↓
migrations add InitialCreate
      ↓
database update
      ↓
SQL Server listo
```

## Modificación posterior

```text
Modificar entidades
      ↓
dotnet build
      ↓
migrations add NombreCambio
      ↓
revisar migration
      ↓
database update
      ↓
BD actualizada
```

## Proyecto descargado de Git

```text
git clone
      ↓
dotnet restore
      ↓
dotnet build
      ↓
database update
      ↓
ejecutar API + MVC
```

---

# Regla principal

```text
¿La migration ya existe en Git?
        │
    ┌───┴───┐
    │       │
   SÍ       NO
    │       │
    │    migrations add
    │       │
    └───┬───┘
        ↓
 database update
```

No crear nuevamente una migration que ya está versionada en el repositorio.


# ¿Qué hay instalado?
dotnet --list-sdks
dotnet --info

# ¿Qué versión utiliza el proyecto?
# Revisar:
<TargetFramework>net7.0</TargetFramework>
<TargetFramework>net8.0</TargetFramework>
<TargetFramework>net9.0</TargetFramework>

# Proyecto existente
dotnet restore
dotnet build

# EF
dotnet ef --version

# SQL/LocalDB
sqlcmd -?
sqllocaldb info

# Primera migración
dotnet ef migrations add InitialCreate --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext

# Aplicar a BD
dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext


--- ultima migracio n a n
dotnet ef migrations add AddProductCategories --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context AppContext

--- aplicamos al migracion 

dotnet ef database update --project CrudPractica.Infrastructure --startup-project CrudPractica.Api --context PollitoDbContext

-----
el create de categorias

{
      name:"teclados"
}
el create de productos ya pude recibir

{
  "name": "Teclado Logitech",
  "description": "Teclado mecánico",
  "price": 280000,
  "quantity": 10,
  "categoryIds": [1, 2, 3]
}