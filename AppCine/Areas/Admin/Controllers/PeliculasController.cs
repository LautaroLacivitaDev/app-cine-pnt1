using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Areas.Admin.Controllers;

/// <summary>ABM de películas con sus géneros (muchos a muchos) y su reparto.</summary>
public class PeliculasController(CineContext db) : AdminController(db)
{
    public async Task<IActionResult> Index() => View(await Db.Peliculas.AsNoTracking()
        .Where(p => p.Activo)
        .Include(p => p.Director)
        .Include(p => p.Generos.Where(g => g.Activo))
        .OrderBy(p => p.Titulo)
        .ToListAsync());

    public async Task<IActionResult> Create()
    {
        await CargarOpcionesAsync([]);
        return View("Form", new Pelicula { FechaEstreno = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pelicula pelicula, int[] generoIds)
    {
        pelicula.Id = 0;
        var generos = await ValidarAsync(pelicula, generoIds);
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync(generoIds);
            return View("Form", pelicula);
        }

        pelicula.Activo = true;
        pelicula.Generos = generos;
        Db.Peliculas.Add(pelicula);
        await Db.SaveChangesAsync();
        Aviso($"Se creó «{pelicula.Titulo}». Ahora podés cargar el reparto.");
        return RedirectToAction(nameof(Edit), new { id = pelicula.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var pelicula = await BuscarAsync(id);
        if (pelicula is null)
        {
            return NotFound();
        }

        await CargarOpcionesAsync(pelicula.Generos.Select(g => g.Id).ToArray(), pelicula);
        return View("Form", pelicula);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pelicula pelicula, int[] generoIds)
    {
        var existente = await BuscarAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        pelicula.Id = id;
        var generos = await ValidarAsync(pelicula, generoIds);
        if (!ModelState.IsValid)
        {
            pelicula.Repartos = existente.Repartos;
            await CargarOpcionesAsync(generoIds, pelicula);
            return View("Form", pelicula);
        }

        pelicula.Activo = true;
        Db.Entry(existente).CurrentValues.SetValues(pelicula);
        // Se conservan los géneros dados de baja que ya tenía: la baja lógica no los desvincula.
        foreach (var genero in existente.Generos.Where(g => g.Activo).ToList())
        {
            existente.Generos.Remove(genero);
        }

        foreach (var genero in generos)
        {
            existente.Generos.Add(genero);
        }
        await Db.SaveChangesAsync();
        Aviso($"Se guardó «{pelicula.Titulo}».");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var pelicula = await Db.Peliculas.FirstOrDefaultAsync(p => p.Id == id && p.Activo);
        if (pelicula is null)
        {
            return NotFound();
        }

        pelicula.Activo = false;
        await Db.SaveChangesAsync();
        Aviso($"Se eliminó «{pelicula.Titulo}».");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarReparto(int id, [Bind(nameof(Reparto.ActorId), nameof(Reparto.Personaje))] Reparto reparto)
    {
        if (!await Db.Peliculas.AnyAsync(p => p.Id == id && p.Activo))
        {
            return NotFound();
        }

        reparto.PeliculaId = id;
        if (!await Db.Actores.AnyAsync(a => a.Id == reparto.ActorId && a.Activo))
        {
            ModelState.AddModelError(nameof(Reparto.ActorId), "Elegí un actor.");
        }
        else if (await Db.Repartos.AnyAsync(r => r.PeliculaId == id && r.ActorId == reparto.ActorId))
        {
            ModelState.AddModelError(nameof(Reparto.ActorId), "Ese actor ya está en el reparto.");
        }

        if (ModelState.IsValid)
        {
            Db.Repartos.Add(reparto);
            await Db.SaveChangesAsync();
            Aviso($"Se agregó el personaje «{reparto.Personaje}».");
        }
        else
        {
            TempData["ErrorReparto"] = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> QuitarReparto(int id, int repartoId)
    {
        var reparto = await Db.Repartos.FirstOrDefaultAsync(r => r.Id == repartoId && r.PeliculaId == id);
        if (reparto is null)
        {
            return NotFound();
        }

        Db.Repartos.Remove(reparto);
        await Db.SaveChangesAsync();
        Aviso($"Se quitó el personaje «{reparto.Personaje}».");
        return RedirectToAction(nameof(Edit), new { id });
    }

    private Task<Pelicula?> BuscarAsync(int id) => Db.Peliculas
        .Include(p => p.Generos)
        .Include(p => p.Repartos).ThenInclude(r => r.Actor)
        .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

    private async Task<List<Genero>> ValidarAsync(Pelicula pelicula, int[] generoIds)
    {
        var generos = await Db.Generos.Where(g => g.Activo && generoIds.Contains(g.Id)).ToListAsync();
        if (generos.Count == 0)
        {
            ModelState.AddModelError("generoIds", "Elegí al menos un género.");
        }

        if (!await Db.Directores.AnyAsync(d => d.Id == pelicula.DirectorId && d.Activo))
        {
            ModelState.AddModelError(nameof(Pelicula.DirectorId), "Elegí un director.");
        }

        if (!await Db.Distribuidoras.AnyAsync(d => d.Id == pelicula.DistribuidoraId && d.Activo))
        {
            ModelState.AddModelError(nameof(Pelicula.DistribuidoraId), "Elegí una distribuidora.");
        }

        return generos;
    }

    private async Task CargarOpcionesAsync(int[] generoIds, Pelicula? pelicula = null)
    {
        ViewBag.Directores = new SelectList(
            await Db.Directores.Where(d => d.Activo).OrderBy(d => d.Apellido).ToListAsync(),
            nameof(Persona.Id), nameof(Persona.NombreCompleto));
        ViewBag.Distribuidoras = new SelectList(
            await Db.Distribuidoras.Where(d => d.Activo).OrderBy(d => d.RazonSocial).ToListAsync(),
            nameof(Distribuidora.Id), nameof(Distribuidora.RazonSocial));
        ViewBag.Generos = await Db.Generos.Where(g => g.Activo).OrderBy(g => g.Nombre).ToListAsync();
        ViewBag.GeneroIds = generoIds;

        if (pelicula is not null)
        {
            var enReparto = pelicula.Repartos.Select(r => r.ActorId).ToList();
            ViewBag.Actores = new SelectList(
                await Db.Actores.Where(a => a.Activo && !enReparto.Contains(a.Id)).OrderBy(a => a.Apellido).ToListAsync(),
                nameof(Persona.Id), nameof(Persona.NombreCompleto));
        }
    }
}
