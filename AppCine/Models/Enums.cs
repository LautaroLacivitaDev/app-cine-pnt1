using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AppCine.Models;

public enum Clasificacion
{
    [Display(Name = "ATP")] ATP,
    [Display(Name = "+13")] Mayores13,
    [Display(Name = "+16")] Mayores16,
    [Display(Name = "+18")] Mayores18
}

public enum Formato
{
    [Display(Name = "2D")] Formato2D,
    [Display(Name = "3D")] Formato3D
}

public enum Idioma
{
    [Display(Name = "Castellano")] Castellano,
    [Display(Name = "Inglés")] Ingles,
    [Display(Name = "Otro")] Otro
}

public enum GeneroPersona
{
    [Display(Name = "Masculino")] Masculino,
    [Display(Name = "Femenino")] Femenino,
    [Display(Name = "No binario")] NoBinario,
    [Display(Name = "Prefiero no decir")] PrefieroNoDecir
}

public static class EnumExtensions
{
    /// <summary>Texto para mostrar: el Display(Name) del valor o, si no tiene, su nombre.</summary>
    public static string Texto(this Enum valor) =>
        valor.GetType().GetField(valor.ToString())?.GetCustomAttribute<DisplayAttribute>()?.GetName()
        ?? valor.ToString();
}
