# App de cine

Implementación del diagrama *«App de cine: diagrama de clases» (ASP.NET MVC + Entity Framework, v2)*:
cartelera del día, detalle de películas con funciones por sede y sala, login simulado y área de
administración. Está hecho **el núcleo obligatorio**; la **fase 2** (compras, entradas, butacas y candy)
no está implementada, como indica el diagrama ("si sobra tiempo").

## Stack

- ASP.NET Core MVC sobre **.NET 10** (SDK 10.0.401)
- **Entity Framework Core 10** con **SQL Server LocalDB** (`(localdb)\MSSQLLocalDB`, base `AppCine`)
- Bootstrap 5 y jQuery Validation (vienen con la plantilla MVC)

## Cómo correrlo

Desde la carpeta `app-cine`:

```bash
dotnet tool restore
dotnet tool run dotnet-ef database update --project AppCine
dotnet run --project AppCine
```

Abrí http://localhost:5185. En modo desarrollo la app también aplica las migraciones al arrancar, así que el
paso de `database update` es opcional. Para empezar de cero: `dotnet tool run dotnet-ef database drop --project AppCine`.

Para entrar a la administración: **Ingresar → Laura Gómez** (es la única con `EsAdmin`).
Sofía Díaz tiene 15 años (pensada para la regla de edad de la fase 2).

## Estructura

```
AppCine/
├── Models/            Entidades del diagrama, enums y view model de la cartelera
├── Data/              CineContext (Fluent API), DatosDeEjemplo (HasData + funciones) y Migrations/
├── Controllers/       Área pública: Home (cartelera), Peliculas (detalle), Cuenta (login simulado)
├── Views/
└── Areas/Admin/       Área de administración (solo rol Admin)
    ├── Controllers/   CrudController<T> genérico + Peliculas y Funciones con su lógica propia
    └── Views/
```

## Qué cubre del diagrama

| Diagrama | Implementación |
|---|---|
| `Persona` abstracta, `Actor` y `Director` | Herencia TPH: tabla `Personas` con columna `Discriminator` |
| `Reparto` | Clase propia (película, actor, personaje). Índice único película + actor |
| `Pelicula` ↔ `Genero` (muchos a muchos) | Tabla intermedia `PeliculaGenero` |
| `Director`, `Distribuidora` → `Pelicula` (1 a muchos) | `DirectorId`, `DistribuidoraId` |
| `Sede` → `Sala` → `Funcion`, `Pelicula` → `Funcion` | Claves foráneas sin borrado en cascada |
| Enums `Clasificacion`, `Formato`, `Idioma`, `GeneroPersona` | Se guardan como entero; se muestran con `[Display]` (`+13`, `3D`…) |
| Validaciones de formulario | Data Annotations (`[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, `[Url]`) con mensajes en español, más `[NoFutura]` para la fecha de nacimiento |
| Regla 1: sin superposición en la sala | `FuncionesController.ValidarReglasAsync`: `HoraFin()` = inicio + duración + 20 min |
| Regla 2: 3D solo en salas con `Soporta3D` | Ídem |
| Regla 3: sin funciones en el pasado | Ídem |
| Regla 4: baja lógica | Eliminar marca `Activo = false`; listas y cartelera muestran solo activos |
| Datos de ejemplo | 2 sedes × 3 salas, 6 películas con géneros, directores y reparto, 3 usuarios (1 admin), funciones para 7 días |

Pantallas: cartelera con filtros (día, sede, género, formato), detalle de película (póster, sinopsis,
clasificación, director, reparto, géneros, tráiler y horarios por sede, por ejemplo `16:30 2D · 20:00 3D SUB`),
login simulado y ABM de películas (con géneros y reparto), funciones, sedes, salas, actores, directores,
géneros, distribuidoras y usuarios.

## Decisiones ante ambigüedades

- **Base de datos:** el diagrama no la nombra; se usa SQL Server LocalDB por ser la habitual con EF en
  Windows y no requerir instalación extra (ya viene con Visual Studio).
- **Funciones de ejemplo:** el catálogo fijo va con `HasData` (queda en la migración), pero las funciones
  dependen de la fecha de hoy, y con `HasData` quedarían viejas en una semana. Se generan con `UseSeeding`
  de EF al migrar (y en cada arranque en desarrollo) **solo si no hay funciones desde hoy**, así la
  cartelera nunca arranca vacía.
- **Métodos calculados:** `NombreCompleto`, `Edad` y `Capacidad` son propiedades de solo lectura (EF no las
  mapea). `HoraFin()` queda como método porque necesita la película cargada y la validación de MVC recorre
  todas las propiedades del modelo recibido.
- **Usuario no tiene `Activo`** en el diagrama: eliminar un usuario lo borra de verdad (en el núcleo nadie
  lo referencia).
- **Reparto no tiene `Activo`:** quitar a un actor del reparto borra esa fila.
- **`Nacionalidad`, `Descripcion`, `ImagenUrl`, `TrailerUrl`, `Localidad` y `Telefono`** son opcionales: el
  diagrama no los pide como obligatorios. Si se cargan, se valida el formato.
- **Género único:** el índice único abarca también los géneros dados de baja, así un nombre no se repite nunca.
- **Superposición:** solo se compara contra funciones activas de la misma sala. Editar una función también
  exige fecha futura (regla 3).
- **Login simulado:** cookie de autenticación de ASP.NET Core con el rol `Admin` si el usuario tiene `EsAdmin`,
  sin contraseña. Si se cambia `EsAdmin` a alguien, el cambio se ve al volver a ingresar.
- **Fecha y hora** se guardan en hora local (`datetime2`), sin zonas horarias: es una app de un solo cine.
- **Números:** la cultura es `es-AR` (fechas y `$ 6.500`), pero con punto decimal para que los inputs
  numéricos del navegador (que envían `7000.50`) se lean bien.
- **Pósters y tráileres de ejemplo:** pósters de `placehold.co` con el título y tráiler como búsqueda en
  YouTube, para no inventar enlaces que podrían no existir.
- **Seguridad:** se fijó `System.Security.Cryptography.Pkcs` 10.0.14 porque la versión que trae
  `Microsoft.Data.SqlClient` (9.0.11) tiene vulnerabilidades conocidas.

## Fase 2 (pendiente)

Se agrega sin tocar el núcleo: `Compra` (con `UsuarioId`), `Entrada` (con `FuncionId`, índice único en
función + fila + butaca), `ProductoCandy` e `ItemCandy`, la regla de edad según clasificación usando `Edad`,
la pantalla "Mis compras" y el ABM de productos de candy.
