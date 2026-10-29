namespace AppCine.Models;

public class CarteleraViewModel
{
    public required DateTime Fecha { get; init; }
    public required IReadOnlyList<DateTime> Dias { get; init; }
    public int? SedeId { get; init; }
    public int? GeneroId { get; init; }
    public Formato? Formato { get; init; }
    public required IReadOnlyList<Sede> Sedes { get; init; }
    public required IReadOnlyList<Genero> Generos { get; init; }
    public required IReadOnlyList<PeliculaEnCartelera> Peliculas { get; init; }
}

/// <summary>Una película con sus funciones del día, agrupadas por sede.</summary>
public record PeliculaEnCartelera(Pelicula Pelicula, IReadOnlyList<IGrouping<Sede, Funcion>> FuncionesPorSede);
