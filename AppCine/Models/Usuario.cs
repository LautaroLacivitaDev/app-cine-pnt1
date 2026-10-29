using System.ComponentModel.DataAnnotations;

namespace AppCine.Models;

/// <summary>Cliente de la app. EsAdmin decide si ve el área de administración.</summary>
public class Usuario : IEntidad
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, ErrorMessage = "El nombre admite hasta 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(50, ErrorMessage = "El apellido admite hasta 50 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Display(Name = "Correo")]
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Fecha de nacimiento")]
    [DataType(DataType.Date)]
    [NoFutura(ErrorMessage = "La fecha de nacimiento no puede ser futura.")]
    public DateTime FechaNacimiento { get; set; }

    [Display(Name = "Teléfono")]
    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    [StringLength(30)]
    public string? Telefono { get; set; }

    [Display(Name = "Género")]
    public GeneroPersona Genero { get; set; }

    [Display(Name = "Es administrador")]
    public bool EsAdmin { get; set; }

    public string NombreCompleto => $"{Nombre} {Apellido}";

    public int Edad
    {
        get
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - FechaNacimiento.Year;
            return FechaNacimiento.Date > hoy.AddYears(-edad) ? edad - 1 : edad;
        }
    }
}

/// <summary>La fecha no puede ser posterior a hoy.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NoFuturaAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is not DateTime fecha || fecha.Date <= DateTime.Today;
}
