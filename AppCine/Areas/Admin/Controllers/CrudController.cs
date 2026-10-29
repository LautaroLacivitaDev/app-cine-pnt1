using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public abstract class AdminController(CineContext db) : Controller
{
    protected CineContext Db { get; } = db;

    protected void Aviso(string mensaje) => TempData["Aviso"] = mensaje;
}

/// <summary>
/// ABM común para las entidades sin relaciones muchos a muchos. Cada controlador define el listado
/// y, si hace falta, las opciones de los combos y las validaciones que dependen de otros datos.
/// Las vistas son Index y Form en la carpeta del controlador.
/// </summary>
public abstract class CrudController<T>(CineContext db) : AdminController(db) where T : class, IEntidad, new()
{
    protected abstract IQueryable<T> Listado();

    protected abstract string Nombre(T entidad);

    protected virtual Task CargarOpcionesAsync() => Task.CompletedTask;

    protected virtual Task ValidarAsync(T entidad) => Task.CompletedTask;

    public async Task<IActionResult> Index() => View(await Listado().AsNoTracking().ToListAsync());

    public async Task<IActionResult> Create()
    {
        await CargarOpcionesAsync();
        return View("Form", new T());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(T entidad)
    {
        entidad.Id = 0;
        return await GuardarAsync(entidad, () => Db.Add(entidad), "creado");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var entidad = await BuscarAsync(id);
        if (entidad is null)
        {
            return NotFound();
        }

        await CargarOpcionesAsync();
        return View("Form", entidad);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, T entidad)
    {
        var existente = await BuscarAsync(id);
        if (existente is null)
        {
            return NotFound();
        }

        entidad.Id = id;
        return await GuardarAsync(entidad, () => Db.Entry(existente).CurrentValues.SetValues(entidad), "actualizado");
    }

    /// <summary>Baja lógica si la entidad la admite (regla 4); si no, se borra.</summary>
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entidad = await BuscarAsync(id);
        if (entidad is null)
        {
            return NotFound();
        }

        if (entidad is IActivable activable)
        {
            activable.Activo = false;
        }
        else
        {
            Db.Remove(entidad);
        }

        await Db.SaveChangesAsync();
        Aviso($"Se eliminó «{Nombre(entidad)}».");
        return RedirectToAction(nameof(Index));
    }

    private async Task<T?> BuscarAsync(int id)
    {
        var entidad = await Db.Set<T>().FindAsync(id);
        return entidad is null or IActivable { Activo: false } ? null : entidad;
    }

    private async Task<IActionResult> GuardarAsync(T entidad, Action registrar, string accion)
    {
        await ValidarAsync(entidad);
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return View("Form", entidad);
        }

        // Solo se crean y editan registros activos; el formulario no envía este campo.
        if (entidad is IActivable activable)
        {
            activable.Activo = true;
        }

        registrar();
        await Db.SaveChangesAsync();
        Aviso($"Se guardó «{Nombre(entidad)}» ({accion}).");
        return RedirectToAction(nameof(Index));
    }
}
