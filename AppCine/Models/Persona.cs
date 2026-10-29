using System.ComponentModel.DataAnnotations;

namespace AppCine.Models;

/// <summary>Clase base abstracta: siempre es un actor o un director (tabla Personas con Discriminator).</summary>
public abstract class Persona : IEntidad, IActivable
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre admite hasta 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, ErrorMessage = "El apellido admite hasta 50 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "La nacionalidad admite hasta 50 caracteres.")]
    public string? Nacionalidad { get; set; }

    public bool Activo { get; set; } = true;

    public string NombreCompleto => $"{Nombre} {Apellido}";
}

public class Actor : Persona
{
    public ICollection<Reparto> Repartos { get; set; } = new List<Reparto>();
}

public class Director : Persona
{
    public ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
}

/// <summary>Une una película con un actor e indica qué personaje interpreta.</summary>
public class Reparto : IEntidad
{
    public int Id { get; set; }

    public int PeliculaId { get; set; }
    public Pelicula? Pelicula { get; set; }

    [Display(Name = "Actor")]
    public int ActorId { get; set; }
    public Actor? Actor { get; set; }

    [Required(ErrorMessage = "El personaje es obligatorio.")]
    [StringLength(80, ErrorMessage = "El personaje admite hasta 80 caracteres.")]
    public string Personaje { get; set; } = string.Empty;
}
