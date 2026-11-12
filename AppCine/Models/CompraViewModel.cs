namespace AppCine.Models;

/// <summary>Pantalla de elección de butacas para una función.</summary>
public class CompraViewModel
{
    public required Funcion Funcion { get; init; }

    /// <summary>Butacas ya vendidas (fila, butaca).</summary>
    public required IReadOnlySet<(int Fila, int Butaca)> Ocupadas { get; init; }

    /// <summary>Butacas que el usuario había marcado, para no perderlas si el formulario vuelve con errores.</summary>
    public required IReadOnlySet<(int Fila, int Butaca)> Elegidas { get; init; }

    /// <summary>Motivo por el que no se puede comprar (edad o función empezada); null si se puede.</summary>
    public string? Bloqueo { get; init; }
}
