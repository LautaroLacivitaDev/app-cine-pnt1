using System.ComponentModel.DataAnnotations;

namespace AppCine.Models;

/// <summary>Título en cartelera con sus datos fijos. El formato y el idioma dependen de cada función.</summary>
public class Pelicula : IEntidad, IActivable
{
    public int Id { get; set; }

    [Display(Name = "Título")]
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título admite hasta 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Sinopsis")]
    [StringLength(2000, ErrorMessage = "La sinopsis admite hasta 2000 caracteres.")]
    public string? Descripcion { get; set; }

    [Display(Name = "Duración (minutos)")]
    [Range(1, 400, ErrorMessage = "La duración tiene que estar entre 1 y 400 minutos.")]
    public int DuracionMinutos { get; set; }

    [Display(Name = "Clasificación")]
    public Clasificacion Clasificacion { get; set; }

    [Display(Name = "Póster (URL)")]
    [Url(ErrorMessage = "La imagen tiene que ser una URL válida.")]
    [StringLength(500)]
    public string? ImagenUrl { get; set; }

    [Display(Name = "Tráiler (URL de YouTube)")]
    [Url(ErrorMessage = "El tráiler tiene que ser una URL válida.")]
    [StringLength(500)]
    public string? TrailerUrl { get; set; }

    [Display(Name = "Fecha de estreno")]
    [DataType(DataType.Date)]
    public DateTime FechaEstreno { get; set; }

    public bool Activo { get; set; } = true;

    [Display(Name = "Director")]
    [Range(1, int.MaxValue, ErrorMessage = "Elegí un director.")]
    public int DirectorId { get; set; }
    public Director? Director { get; set; }

    [Display(Name = "Distribuidora")]
    [Range(1, int.MaxValue, ErrorMessage = "Elegí una distribuidora.")]
    public int DistribuidoraId { get; set; }
    public Distribuidora? Distribuidora { get; set; }

    public ICollection<Genero> Generos { get; set; } = new List<Genero>();
    public ICollection<Reparto> Repartos { get; set; } = new List<Reparto>();
    public ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();
}

/// <summary>Categoría de la película. Una película puede tener varias.</summary>
public class Genero : IEntidad, IActivable
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre admite hasta 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
}

public class Distribuidora : IEntidad, IActivable
{
    public int Id { get; set; }

    [Display(Name = "Razón social")]
    [Required(ErrorMessage = "La razón social es obligatoria.")]
    [StringLength(100, ErrorMessage = "La razón social admite hasta 100 caracteres.")]
    public string RazonSocial { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
}
