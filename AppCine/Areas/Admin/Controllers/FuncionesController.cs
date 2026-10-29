using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Areas.Admin.Controllers;

/// <summary>ABM de funciones. Aplica las reglas de negocio 1 a 3 antes de guardar.</summary>
public class FuncionesController(CineContext db) : AdminController(db)
{
    public async Task<IActionResult> Index(bool pasadas = false)
    {
        var ahora = DateTime.Now;
        var funciones = Db.Funciones.AsNoTracking()
            .Where(f => f.Activo)
            .Where(f => pasadas ? f.FechaHora < ahora : f.FechaHora >= ahora)
            .Include(f => f.Pelicula)
            .Include(f => f.Sala).ThenInclude(s => s!.Sede);
        ViewBag.Pasadas = pasadas;
        return View(await (pasadas ? funciones.OrderByDescending(f => f.FechaHora) : funciones.OrderBy(f => f.FechaHora))
            .Take(300)
            .ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarOpcionesAsync();
        return View("Form", new Funcion { FechaHora = DateTime.Today.AddDays(1).AddHours(20), Precio = 6500 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Funcion funcion)
    {
        funcion.Id = 0;
        await ValidarReglasAsync(funcion);
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return View("Form", funcion);
        }

        funcion.Activo = true;
        Db.Funciones.Add(funcion);
        await Db.SaveChangesAsync();
        Aviso("Se creó la función.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var funcion = await Db.Funciones.FirstOrDefaultAsync(f => f.Id == id && f.Activo);
        if (funcion is null)
        {
            return NotFound();
        }

        await CargarOpcionesAsync();
        return View("Form", funcion);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Funcion funcion)
    {
        var existente = await Db.Funciones.FirstOrDefaultAsync(f => f.Id == id && f.Activo);
        if (existente is null)
        {
            return NotFound();
        }

        funcion.Id = id;
        await ValidarReglasAsync(funcion);
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return View("Form", funcion);
        }

        funcion.Activo = true;
        Db.Entry(existente).CurrentValues.SetValues(funcion);
        await Db.SaveChangesAsync();
        Aviso("Se guardó la función.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var funcion = await Db.Funciones.FirstOrDefaultAsync(f => f.Id == id && f.Activo);
        if (funcion is null)
        {
            return NotFound();
        }

        funcion.Activo = false;
        await Db.SaveChangesAsync();
        Aviso("Se eliminó la función.");
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidarReglasAsync(Funcion funcion)
    {
        var pelicula = await Db.Peliculas.FirstOrDefaultAsync(p => p.Id == funcion.PeliculaId && p.Activo);
        var sala = await Db.Salas.FirstOrDefaultAsync(s => s.Id == funcion.SalaId && s.Activo && s.Sede!.Activo);
        if (pelicula is null)
        {
            ModelState.AddModelError(nameof(Funcion.PeliculaId), "Elegí una película activa.");
        }

        if (sala is null)
        {
            ModelState.AddModelError(nameof(Funcion.SalaId), "Elegí una sala activa.");
        }

        // Regla 3: sin funciones en el pasado.
        if (funcion.FechaHora <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(Funcion.FechaHora), "La fecha y hora tiene que ser posterior a este momento.");
        }

        // Regla 2: 3D solo en salas preparadas.
        if (sala is not null && funcion.Formato == Formato.Formato3D && !sala.Soporta3D)
        {
            ModelState.AddModelError(nameof(Funcion.Formato), $"La {sala.Nombre} no está preparada para 3D.");
        }

        // Regla 1: sin superposición en la sala (duración de la película más 20 minutos de limpieza).
        if (pelicula is not null && sala is not null)
        {
            funcion.Pelicula = pelicula;
            var inicio = funcion.FechaHora;
            var fin = funcion.HoraFin();
            // Ninguna función dura más de un día (400 min + limpieza), así alcanza con mirar ese margen.
            var cercanas = await Db.Funciones.AsNoTracking()
                .Where(f => f.Activo && f.SalaId == sala.Id && f.Id != funcion.Id
                            && f.FechaHora > inicio.AddDays(-1) && f.FechaHora < fin)
                .Include(f => f.Pelicula)
                .ToListAsync();
            var pisada = cercanas.FirstOrDefault(f => f.FechaHora < fin && f.HoraFin() > inicio);
            if (pisada is not null)
            {
                ModelState.AddModelError(nameof(Funcion.FechaHora),
                    $"Se superpone con «{pisada.Pelicula!.Titulo}» de {pisada.FechaHora:dd/MM HH:mm} a {pisada.HoraFin():HH:mm} en la misma sala.");
            }

            funcion.Pelicula = null;
        }
    }

    private async Task CargarOpcionesAsync()
    {
        ViewBag.Peliculas = new SelectList(
            await Db.Peliculas.Where(p => p.Activo).OrderBy(p => p.Titulo).ToListAsync(),
            nameof(Pelicula.Id), nameof(Pelicula.Titulo));
        var salas = await Db.Salas.Where(s => s.Activo && s.Sede!.Activo).Include(s => s.Sede)
            .OrderBy(s => s.Sede!.Nombre).ThenBy(s => s.Nombre).ToListAsync();
        ViewBag.Salas = salas.Select(s => new SelectListItem(
            $"{s.Sede!.Nombre} · {s.Nombre}{(s.Soporta3D ? " (3D)" : "")}", s.Id.ToString()));
    }
}
