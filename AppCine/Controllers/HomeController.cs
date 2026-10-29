using System.Diagnostics;
using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Controllers;

public class HomeController(CineContext db) : Controller
{
    /// <summary>Días que muestra la cartelera: hoy y los 6 siguientes.</summary>
    private const int DiasEnCartelera = 7;

    /// <summary>Cartelera: películas con funciones en la fecha elegida, filtradas por sede, género y formato.</summary>
    public async Task<IActionResult> Index(DateTime? fecha, int? sedeId, int? generoId, Formato? formato)
    {
        var hoy = DateTime.Today;
        var dia = fecha?.Date is { } elegida && elegida >= hoy && elegida < hoy.AddDays(DiasEnCartelera) ? elegida : hoy;
        // Hoy solo se muestran las funciones que todavía no empezaron.
        var desde = dia == hoy ? DateTime.Now : dia;
        var hasta = dia.AddDays(1);

        // Con resolución de identidad, cada película y sede es una sola instancia y se puede agrupar por ella.
        var consulta = db.Funciones.AsNoTrackingWithIdentityResolution()
            .Where(f => f.Activo && f.FechaHora >= desde && f.FechaHora < hasta)
            .Where(f => f.Pelicula!.Activo && f.Sala!.Activo && f.Sala.Sede!.Activo);
        if (sedeId is not null)
        {
            consulta = consulta.Where(f => f.Sala!.SedeId == sedeId);
        }

        if (generoId is not null)
        {
            consulta = consulta.Where(f => f.Pelicula!.Generos.Any(g => g.Id == generoId));
        }

        if (formato is not null)
        {
            consulta = consulta.Where(f => f.Formato == formato);
        }

        var funciones = await consulta
            .Include(f => f.Pelicula).ThenInclude(p => p!.Generos.Where(g => g.Activo))
            .Include(f => f.Sala).ThenInclude(s => s!.Sede)
            .OrderBy(f => f.FechaHora)
            .ToListAsync();

        var peliculas = funciones
            .GroupBy(f => f.Pelicula!)
            .Select(g => new PeliculaEnCartelera(
                g.Key,
                g.GroupBy(f => f.Sala!.Sede!).OrderBy(s => s.Key.Nombre).ToList()))
            .OrderBy(p => p.Pelicula.Titulo)
            .ToList();

        return View(new CarteleraViewModel
        {
            Fecha = dia,
            Dias = Enumerable.Range(0, DiasEnCartelera).Select(n => hoy.AddDays(n)).ToList(),
            SedeId = sedeId,
            GeneroId = generoId,
            Formato = formato,
            Sedes = await db.Sedes.AsNoTracking().Where(s => s.Activo).OrderBy(s => s.Nombre).ToListAsync(),
            Generos = await db.Generos.AsNoTracking().Where(g => g.Activo).OrderBy(g => g.Nombre).ToListAsync(),
            Peliculas = peliculas
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
