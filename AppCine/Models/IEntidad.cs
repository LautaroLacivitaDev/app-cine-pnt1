namespace AppCine.Models;

public interface IEntidad
{
    int Id { get; set; }
}

/// <summary>Entidades con baja lógica: eliminar marca Activo = false (regla de negocio 4).</summary>
public interface IActivable
{
    bool Activo { get; set; }
}
