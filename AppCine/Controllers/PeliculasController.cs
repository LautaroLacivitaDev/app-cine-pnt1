using AppCine.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Controllers;

public class PeliculasController(CineContext db) : Controller
{
    /// <summary>Detalle: póster, sinopsis, reparto, géneros, tráiler y horarios de la semana agrupados por sede.</summary>
    public async Task<IActionResult> Detalle(int id)
    {
        var pelicula = await db.Peliculas.AsNoTracking()
            .Include(p => p.Director)
            .Include(p => p.Distribuidora)
            .Include(p => p.Generos.Where(g => g.Activo))
            .Include(p => p.Repartos.Where(r => r.Actor!.Activo)).ThenInclude(r => r.Actor)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);
        if (pelicula is null)
        {
            return NotFound();
        }

        var ahora = DateTime.Now;
        var hasta = DateTime.Today.AddDays(7);
        // Con resolución de identidad, cada sede es una sola instancia y se puede agrupar por ella.
        ViewBag.FuncionesPorSede = (await db.Funciones.AsNoTrackingWithIdentityResolution()
                .Where(f => f.PeliculaId == id && f.Activo && f.FechaHora >= ahora && f.FechaHora < hasta)
                .Where(f => f.Sala!.Activo && f.Sala.Sede!.Activo)
                .Include(f => f.Sala).ThenInclude(s => s!.Sede)
                .OrderBy(f => f.FechaHora)
                .ToListAsync())
            .GroupBy(f => f.Sala!.Sede!)
            .OrderBy(g => g.Key.Nombre)
            .ToList();

        return View(pelicula);
    }
}
