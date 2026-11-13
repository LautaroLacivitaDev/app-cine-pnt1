using System.Security.Claims;
using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Controllers;

/// <summary>Compra de entradas (fase 2, sin candy todavía). Solo para usuarios que ingresaron.</summary>
[Authorize]
public class ComprasController(CineContext db) : Controller
{
    /// <summary>Mis compras: el historial del usuario que ingresó, de la más nueva a la más vieja.</summary>
    public async Task<IActionResult> Index()
    {
        var usuarioId = UsuarioId();
        var compras = await db.Compras.AsNoTracking()
            .Where(c => c.UsuarioId == usuarioId)
            .Include(c => c.Entradas).ThenInclude(e => e.Funcion).ThenInclude(f => f!.Pelicula)
            .Include(c => c.Entradas).ThenInclude(e => e.Funcion).ThenInclude(f => f!.Sala).ThenInclude(s => s!.Sede)
            .OrderByDescending(c => c.FechaHora)
            .AsSplitQuery()
            .ToListAsync();
        return View(compras);
    }

    /// <summary>Comprobante de una compra. Cada usuario ve solo las suyas.</summary>
    public async Task<IActionResult> Detalle(int id)
    {
        var usuarioId = UsuarioId();
        var compra = await db.Compras.AsNoTracking()
            .Include(c => c.Entradas).ThenInclude(e => e.Funcion).ThenInclude(f => f!.Pelicula)
            .Include(c => c.Entradas).ThenInclude(e => e.Funcion).ThenInclude(f => f!.Sala).ThenInclude(s => s!.Sede)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
        return compra is null ? NotFound() : View(compra);
    }

    /// <summary>Mapa de la sala para elegir butacas. El id es el de la función.</summary>
    public async Task<IActionResult> Nueva(int id)
    {
        var funcion = await BuscarFuncionAsync(id);
        if (funcion is null)
        {
            return NotFound();
        }

        var usuario = await UsuarioActualAsync();
        if (usuario is null)
        {
            return Challenge();
        }

        return View(new CompraViewModel
        {
            Funcion = funcion,
            Ocupadas = await OcupadasAsync(funcion.Id),
            Elegidas = new HashSet<(int, int)>(),
            Bloqueo = Bloqueo(funcion, usuario)
        });
    }

    /// <summary>
    /// Confirma la compra. Cada butaca llega como "fila-butaca" (por ejemplo "3-12").
    /// Compra y entradas se guardan en un solo SaveChanges, así que se graban todas o ninguna.
    /// </summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Nueva(int id, string[]? butacas)
    {
        var funcion = await BuscarFuncionAsync(id);
        if (funcion is null)
        {
            return NotFound();
        }

        var usuario = await UsuarioActualAsync();
        if (usuario is null)
        {
            return Challenge();
        }

        var bloqueo = Bloqueo(funcion, usuario);
        var ocupadas = await OcupadasAsync(funcion.Id);
        var elegidas = LeerButacas(butacas ?? [], funcion.Sala!);

        if (elegidas is null)
        {
            ModelState.AddModelError("butacas", "Alguna de las butacas elegidas no existe en esta sala.");
            elegidas = [];
        }
        else if (elegidas.Count == 0)
        {
            ModelState.AddModelError("butacas", "Elegí al menos una butaca.");
        }
        else if (elegidas.Count > Compra.MaximoEntradas)
        {
            ModelState.AddModelError("butacas", $"Podés comprar hasta {Compra.MaximoEntradas} entradas por vez.");
        }

        var yaVendidas = elegidas.Intersect(ocupadas).OrderBy(b => b.Fila).ThenBy(b => b.Butaca).ToList();
        if (yaVendidas.Count > 0)
        {
            ModelState.AddModelError("butacas", "Ya se vendieron: "
                + string.Join("; ", yaVendidas.Select(b => $"fila {b.Fila}, butaca {b.Butaca}")) + ".");
        }

        if (bloqueo is not null || !ModelState.IsValid)
        {
            elegidas.ExceptWith(ocupadas);
            return View(new CompraViewModel { Funcion = funcion, Ocupadas = ocupadas, Elegidas = elegidas, Bloqueo = bloqueo });
        }

        var compra = new Compra
        {
            UsuarioId = usuario.Id,
            FechaHora = DateTime.Now,
            Entradas = elegidas
                .OrderBy(b => b.Fila).ThenBy(b => b.Butaca)
                .Select(b => new Entrada { FuncionId = funcion.Id, Fila = b.Fila, Butaca = b.Butaca, Precio = funcion.Precio })
                .ToList()
        };
        compra.Total = compra.Entradas.Sum(e => e.Precio);
        db.Compras.Add(compra);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Otra persona compró alguna de estas butacas entre la validación y el guardado:
            // el índice único (función, fila, butaca) rechaza la inserción y no se graba nada.
            db.ChangeTracker.Clear();
            ModelState.AddModelError("butacas", "Alguna de las butacas se acaba de vender. Elegí otras.");
            var actualizadas = await OcupadasAsync(funcion.Id);
            elegidas.ExceptWith(actualizadas);
            return View(new CompraViewModel { Funcion = funcion, Ocupadas = actualizadas, Elegidas = elegidas });
        }

        TempData["Aviso"] = compra.Entradas.Count == 1
            ? $"¡Listo! Compraste 1 entrada para «{funcion.Pelicula!.Titulo}»."
            : $"¡Listo! Compraste {compra.Entradas.Count} entradas para «{funcion.Pelicula!.Titulo}».";
        return RedirectToAction(nameof(Detalle), new { id = compra.Id });
    }

    /// <summary>Función activa con su película, sala y sede activas (las mismas condiciones que la cartelera).</summary>
    private Task<Funcion?> BuscarFuncionAsync(int id) => db.Funciones.AsNoTracking()
        .Include(f => f.Pelicula)
        .Include(f => f.Sala).ThenInclude(s => s!.Sede)
        .FirstOrDefaultAsync(f => f.Id == id && f.Activo
                                  && f.Pelicula!.Activo && f.Sala!.Activo && f.Sala.Sede!.Activo);

    /// <summary>Reglas que impiden comprar cualquier butaca de la función.</summary>
    private static string? Bloqueo(Funcion funcion, Usuario usuario)
    {
        if (funcion.FechaHora <= DateTime.Now)
        {
            return "Esta función ya empezó. Elegí otro horario.";
        }

        // Regla de edad: la clasificación de la película fija la edad mínima del comprador.
        var pelicula = funcion.Pelicula!;
        var minima = pelicula.Clasificacion.EdadMinima();
        if (usuario.Edad < minima)
        {
            return $"«{pelicula.Titulo}» es {pelicula.Clasificacion.Texto()}: hace falta tener {minima} años o más "
                   + $"y tenés {usuario.Edad}.";
        }

        return null;
    }

    private async Task<HashSet<(int Fila, int Butaca)>> OcupadasAsync(int funcionId) =>
        (await db.Entradas.AsNoTracking()
            .Where(e => e.FuncionId == funcionId)
            .Select(e => new { e.Fila, e.Butaca })
            .ToListAsync())
        .Select(e => (e.Fila, e.Butaca))
        .ToHashSet();

    /// <summary>Convierte "fila-butaca" en pares. Devuelve null si alguno no es válido o no existe en la sala.</summary>
    private static HashSet<(int Fila, int Butaca)>? LeerButacas(IEnumerable<string> valores, Sala sala)
    {
        var resultado = new HashSet<(int Fila, int Butaca)>();
        foreach (var valor in valores)
        {
            var partes = valor.Split('-');
            if (partes.Length != 2
                || !int.TryParse(partes[0], out var fila) || !int.TryParse(partes[1], out var butaca)
                || fila < 1 || fila > sala.CantidadFilas || butaca < 1 || butaca > sala.ButacasPorFila)
            {
                return null;
            }

            resultado.Add((fila, butaca));
        }

        return resultado;
    }

    private int UsuarioId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    /// <summary>El usuario de la cookie, si sigue existiendo y activo.</summary>
    private Task<Usuario?> UsuarioActualAsync()
    {
        var id = UsuarioId();
        return db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && u.Activo);
    }
}
