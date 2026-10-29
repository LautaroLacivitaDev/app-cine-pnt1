using AppCine.Models;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Data;

/// <summary>
/// Datos de ejemplo: catálogo fijo con HasData (va en la migración) y funciones para los próximos
/// 7 días, que se generan al migrar (UseSeeding) porque dependen de la fecha del día.
/// </summary>
public static class DatosDeEjemplo
{
    private const string Uip = "United International Pictures S.R.L.";

    public static readonly object[] PeliculaGeneros =
    [
        new { PeliculaId = 1, GeneroId = 1 }, new { PeliculaId = 1, GeneroId = 3 },
        new { PeliculaId = 2, GeneroId = 1 }, new { PeliculaId = 2, GeneroId = 2 },
        new { PeliculaId = 3, GeneroId = 3 }, new { PeliculaId = 3, GeneroId = 1 },
        new { PeliculaId = 4, GeneroId = 1 },
        new { PeliculaId = 5, GeneroId = 4 }, new { PeliculaId = 5, GeneroId = 2 },
        new { PeliculaId = 6, GeneroId = 5 }, new { PeliculaId = 6, GeneroId = 1 }
    ];

    // Películas habladas en otro idioma: alternan funciones en inglés subtituladas y dobladas.
    private static readonly HashSet<int> PeliculasExtranjeras = [5, 6];

    public static void Cargar(ModelBuilder mb)
    {
        mb.Entity<Genero>().HasData(
            new Genero { Id = 1, Nombre = "Drama" },
            new Genero { Id = 2, Nombre = "Comedia" },
            new Genero { Id = 3, Nombre = "Suspenso" },
            new Genero { Id = 4, Nombre = "Animación" },
            new Genero { Id = 5, Nombre = "Ciencia ficción" });

        mb.Entity<Distribuidora>().HasData(
            new Distribuidora { Id = 1, RazonSocial = Uip },
            new Distribuidora { Id = 2, RazonSocial = "Distribuidora Andes S.A." },
            new Distribuidora { Id = 3, RazonSocial = "Cine Sur Films S.R.L." });

        mb.Entity<Director>().HasData(
            new Director { Id = 1, Nombre = "Juan José", Apellido = "Campanella", Nacionalidad = "Argentina" },
            new Director { Id = 2, Nombre = "Damián", Apellido = "Szifron", Nacionalidad = "Argentina" },
            new Director { Id = 3, Nombre = "Fabián", Apellido = "Bielinsky", Nacionalidad = "Argentina" },
            new Director { Id = 4, Nombre = "Santiago", Apellido = "Mitre", Nacionalidad = "Argentina" },
            new Director { Id = 5, Nombre = "John", Apellido = "Lasseter", Nacionalidad = "Estados Unidos" },
            new Director { Id = 6, Nombre = "Christopher", Apellido = "Nolan", Nacionalidad = "Reino Unido" });

        mb.Entity<Actor>().HasData(
            new Actor { Id = 10, Nombre = "Ricardo", Apellido = "Darín", Nacionalidad = "Argentina" },
            new Actor { Id = 11, Nombre = "Soledad", Apellido = "Villamil", Nacionalidad = "Argentina" },
            new Actor { Id = 12, Nombre = "Érica", Apellido = "Rivas", Nacionalidad = "Argentina" },
            new Actor { Id = 13, Nombre = "Gastón", Apellido = "Pauls", Nacionalidad = "Argentina" },
            new Actor { Id = 14, Nombre = "Peter", Apellido = "Lanzani", Nacionalidad = "Argentina" },
            new Actor { Id = 15, Nombre = "Tom", Apellido = "Hanks", Nacionalidad = "Estados Unidos" },
            new Actor { Id = 16, Nombre = "Tim", Apellido = "Allen", Nacionalidad = "Estados Unidos" },
            new Actor { Id = 17, Nombre = "Matthew", Apellido = "McConaughey", Nacionalidad = "Estados Unidos" },
            new Actor { Id = 18, Nombre = "Anne", Apellido = "Hathaway", Nacionalidad = "Estados Unidos" });

        mb.Entity<Pelicula>().HasData(
            Pelicula(1, "El secreto de sus ojos", 129, Clasificacion.Mayores16, new DateTime(2009, 8, 13), 1, 1,
                "Un empleado judicial retirado escribe una novela sobre un caso de asesinato que nunca pudo cerrar."),
            Pelicula(2, "Relatos salvajes", 122, Clasificacion.Mayores16, new DateTime(2014, 8, 21), 2, 2,
                "Seis historias independientes de personas que pierden el control frente a la injusticia cotidiana."),
            Pelicula(3, "Nueve reinas", 114, Clasificacion.Mayores13, new DateTime(2000, 8, 31), 3, 3,
                "Dos estafadores se cruzan por casualidad y arman en un día el golpe de sus vidas."),
            Pelicula(4, "Argentina, 1985", 140, Clasificacion.Mayores13, new DateTime(2022, 9, 29), 4, 1,
                "Los fiscales Strassera y Moreno Ocampo llevan adelante el Juicio a las Juntas."),
            Pelicula(5, "Toy Story", 81, Clasificacion.ATP, new DateTime(1995, 12, 21), 5, 2,
                "Los juguetes de Andy cobran vida cuando nadie los ve, y la llegada de Buzz lo cambia todo."),
            Pelicula(6, "Interestelar", 169, Clasificacion.Mayores13, new DateTime(2014, 11, 6), 6, 1,
                "Un grupo de exploradores viaja a través de un agujero de gusano en busca de un nuevo hogar."));

        mb.Entity<Reparto>().HasData(
            new Reparto { Id = 1, PeliculaId = 1, ActorId = 10, Personaje = "Benjamín Espósito" },
            new Reparto { Id = 2, PeliculaId = 1, ActorId = 11, Personaje = "Irene Menéndez Hastings" },
            new Reparto { Id = 3, PeliculaId = 2, ActorId = 10, Personaje = "Simón Fischer" },
            new Reparto { Id = 4, PeliculaId = 2, ActorId = 12, Personaje = "Romina" },
            new Reparto { Id = 5, PeliculaId = 3, ActorId = 10, Personaje = "Marcos" },
            new Reparto { Id = 6, PeliculaId = 3, ActorId = 13, Personaje = "Juan" },
            new Reparto { Id = 7, PeliculaId = 4, ActorId = 10, Personaje = "Julio César Strassera" },
            new Reparto { Id = 8, PeliculaId = 4, ActorId = 14, Personaje = "Luis Moreno Ocampo" },
            new Reparto { Id = 9, PeliculaId = 5, ActorId = 15, Personaje = "Woody" },
            new Reparto { Id = 10, PeliculaId = 5, ActorId = 16, Personaje = "Buzz Lightyear" },
            new Reparto { Id = 11, PeliculaId = 6, ActorId = 17, Personaje = "Cooper" },
            new Reparto { Id = 12, PeliculaId = 6, ActorId = 18, Personaje = "Amelia Brand" });

        mb.Entity<Sede>().HasData(
            new Sede { Id = 1, Nombre = "Sede Centro", Direccion = "Av. Corrientes 1500", Localidad = "CABA" },
            new Sede { Id = 2, Nombre = "Sede Norte", Direccion = "Av. Maipú 2200", Localidad = "Vicente López" });

        mb.Entity<Sala>().HasData(
            new Sala { Id = 1, SedeId = 1, Nombre = "Sala 1", CantidadFilas = 12, ButacasPorFila = 20, Soporta3D = true },
            new Sala { Id = 2, SedeId = 1, Nombre = "Sala 2", CantidadFilas = 10, ButacasPorFila = 16 },
            new Sala { Id = 3, SedeId = 1, Nombre = "Sala 3", CantidadFilas = 8, ButacasPorFila = 14 },
            new Sala { Id = 4, SedeId = 2, Nombre = "Sala 1", CantidadFilas = 14, ButacasPorFila = 22, Soporta3D = true },
            new Sala { Id = 5, SedeId = 2, Nombre = "Sala 2", CantidadFilas = 10, ButacasPorFila = 18 },
            new Sala { Id = 6, SedeId = 2, Nombre = "Sala 3", CantidadFilas = 9, ButacasPorFila = 15 });

        mb.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1, Nombre = "Laura", Apellido = "Gómez", Email = "laura.gomez@example.com",
                FechaNacimiento = new DateTime(1990, 3, 14), Telefono = "11 5555-0001",
                Genero = GeneroPersona.Femenino, EsAdmin = true
            },
            new Usuario
            {
                Id = 2, Nombre = "Martín", Apellido = "Pérez", Email = "martin.perez@example.com",
                FechaNacimiento = new DateTime(1985, 7, 2), Telefono = "11 5555-0002",
                Genero = GeneroPersona.Masculino
            },
            new Usuario
            {
                Id = 3, Nombre = "Sofía", Apellido = "Díaz", Email = "sofia.diaz@example.com",
                FechaNacimiento = new DateTime(2011, 5, 10), Telefono = "11 5555-0003",
                Genero = GeneroPersona.PrefieroNoDecir
            });
    }

    /// <summary>
    /// Genera funciones para hoy y los 6 días siguientes si no hay ninguna desde hoy, así la cartelera
    /// nunca arranca vacía. Tres horarios por sala separados 3 h 30 min: no se pisan ni con la película más larga.
    /// </summary>
    public static void GenerarFunciones(DbContext db)
    {
        var funciones = db.Set<Funcion>();
        var hoy = DateTime.Today;
        if (funciones.Any(f => f.Activo && f.FechaHora >= hoy))
        {
            return;
        }

        var peliculas = db.Set<Pelicula>().Where(p => p.Activo).OrderBy(p => p.Id).ToList();
        var salas = db.Set<Sala>().Where(s => s.Activo).OrderBy(s => s.Id).ToList();
        if (peliculas.Count == 0)
        {
            return;
        }

        TimeSpan[] horarios = [new(13, 0, 0), new(16, 30, 0), new(20, 0, 0)];
        for (var dia = 0; dia < 7; dia++)
        {
            for (var s = 0; s < salas.Count; s++)
            {
                for (var h = 0; h < horarios.Length; h++)
                {
                    var pelicula = peliculas[(s + h + dia) % peliculas.Count];
                    var es3D = salas[s].Soporta3D && h == horarios.Length - 1;
                    var subtitulada = PeliculasExtranjeras.Contains(pelicula.Id) && (h + dia) % 2 == 0;
                    funciones.Add(new Funcion
                    {
                        PeliculaId = pelicula.Id,
                        SalaId = salas[s].Id,
                        FechaHora = hoy.AddDays(dia).Add(horarios[h]),
                        Formato = es3D ? Formato.Formato3D : Formato.Formato2D,
                        Idioma = subtitulada ? Idioma.Ingles : Idioma.Castellano,
                        Subtitulada = subtitulada,
                        Precio = es3D ? 8500m : 6500m
                    });
                }
            }
        }

        db.SaveChanges();
    }

    private static Pelicula Pelicula(int id, string titulo, int minutos, Clasificacion clasificacion,
        DateTime estreno, int directorId, int distribuidoraId, string descripcion) => new()
    {
        Id = id,
        Titulo = titulo,
        Descripcion = descripcion,
        DuracionMinutos = minutos,
        Clasificacion = clasificacion,
        FechaEstreno = estreno,
        DirectorId = directorId,
        DistribuidoraId = distribuidoraId,
        ImagenUrl = $"https://placehold.co/400x600/1f2937/f9fafb/png?text={Uri.EscapeDataString(titulo)}",
        TrailerUrl = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(titulo + " trailer")}"
    };
}
