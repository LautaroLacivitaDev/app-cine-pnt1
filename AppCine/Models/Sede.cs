using System.ComponentModel.DataAnnotations;

namespace AppCine.Models;

/// <summary>Complejo de cines con dirección propia.</summary>
public class Sede : IEntidad, IActivable
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre admite hasta 80 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Dirección")]
    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(150, ErrorMessage = "La dirección admite hasta 150 caracteres.")]
    public string Direccion { get; set; } = string.Empty;

    [StringLength(80, ErrorMessage = "La localidad admite hasta 80 caracteres.")]
    public string? Localidad { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Sala> Salas { get; set; } = new List<Sala>();
}

/// <summary>Sala dentro de una sede. Filas por butacas definen la capacidad.</summary>
public class Sala : IEntidad, IActivable
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre admite hasta 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Cantidad de filas")]
    [Range(1, 30, ErrorMessage = "Las filas tienen que estar entre 1 y 30.")]
    public int CantidadFilas { get; set; }

    [Display(Name = "Butacas por fila")]
    [Range(1, 40, ErrorMessage = "Las butacas por fila tienen que estar entre 1 y 40.")]
    public int ButacasPorFila { get; set; }

    [Display(Name = "Soporta 3D")]
    public bool Soporta3D { get; set; }

    public bool Activo { get; set; } = true;

    [Display(Name = "Sede")]
    [Range(1, int.MaxValue, ErrorMessage = "Elegí una sede.")]
    public int SedeId { get; set; }
    public Sede? Sede { get; set; }

    public ICollection<Funcion> Funciones { get; set; } = new List<Funcion>();

    public int Capacidad => CantidadFilas * ButacasPorFila;
}

/// <summary>Una proyección concreta: película, sala, fecha y hora, formato, idioma y precio.</summary>
public class Funcion : IEntidad, IActivable
{
    /// <summary>Minutos de limpieza de la sala que se suman al final de cada función.</summary>
    public const int MinutosLimpieza = 20;

    public int Id { get; set; }

    [Display(Name = "Fecha y hora")]
    [DataType(DataType.DateTime)]
    public DateTime FechaHora { get; set; }

    public Formato Formato { get; set; }

    public Idioma Idioma { get; set; }

    public bool Subtitulada { get; set; }

    [Range(0.01, 10_000_000, ErrorMessage = "El precio tiene que ser mayor a 0.")]
    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;

    [Display(Name = "Película")]
    [Range(1, int.MaxValue, ErrorMessage = "Elegí una película.")]
    public int PeliculaId { get; set; }
    public Pelicula? Pelicula { get; set; }

    [Display(Name = "Sala")]
    [Range(1, int.MaxValue, ErrorMessage = "Elegí una sala.")]
    public int SalaId { get; set; }
    public Sala? Sala { get; set; }

    /// <summary>
    /// Inicio más la duración de la película y la limpieza. Requiere la película cargada; por eso es un
    /// método y no una propiedad (la validación de MVC recorre las propiedades del modelo recibido).
    /// </summary>
    public DateTime HoraFin() => FechaHora.AddMinutes(
        (Pelicula ?? throw new InvalidOperationException("HoraFin necesita la película cargada.")).DuracionMinutos
        + MinutosLimpieza);

    /// <summary>Texto corto para la cartelera, por ejemplo "19:00 3D SUB".</summary>
    public string Etiqueta => $"{FechaHora:HH:mm} {Formato.Texto()}{(Subtitulada ? " SUB" : "")}";
}
