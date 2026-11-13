using System.ComponentModel.DataAnnotations;

namespace AppCine.Models;

/// <summary>Compra de un usuario. En esta etapa solo tiene entradas; el candy se agrega después.</summary>
public class Compra : IEntidad
{
    /// <summary>Máximo de entradas que se pueden comprar de una vez.</summary>
    public const int MaximoEntradas = 10;

    public int Id { get; set; }

    [Display(Name = "Fecha de compra")]
    public DateTime FechaHora { get; set; }

    /// <summary>Se guarda al comprar para que no cambie si después cambia el precio de la función.</summary>
    public decimal Total { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
}

/// <summary>Una butaca de una función. El índice único función + fila + butaca impide venderla dos veces.</summary>
public class Entrada : IEntidad
{
    public int Id { get; set; }

    public int CompraId { get; set; }
    public Compra? Compra { get; set; }

    public int FuncionId { get; set; }
    public Funcion? Funcion { get; set; }

    /// <summary>Número de fila, desde 1.</summary>
    public int Fila { get; set; }

    /// <summary>Número de butaca dentro de la fila, desde 1.</summary>
    public int Butaca { get; set; }

    /// <summary>Precio de la función al momento de comprar.</summary>
    public decimal Precio { get; set; }

    public string Ubicacion => $"Fila {Fila}, butaca {Butaca}";
}
