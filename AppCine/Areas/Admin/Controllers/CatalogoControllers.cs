using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Areas.Admin.Controllers;

public class InicioController(CineContext db) : AdminController(db)
{
    public IActionResult Index() => View();
}

public class GenerosController(CineContext db) : CrudController<Genero>(db)
{
    protected override IQueryable<Genero> Listado() => Db.Generos.Where(g => g.Activo).OrderBy(g => g.Nombre);

    protected override string Nombre(Genero genero) => genero.Nombre;

    // El índice único abarca también los dados de baja: un nombre no se repite nunca.
    protected override async Task ValidarAsync(Genero genero)
    {
        if (await Db.Generos.AnyAsync(g => g.Nombre == genero.Nombre && g.Id != genero.Id))
        {
            ModelState.AddModelError(nameof(Genero.Nombre), "Ya existe un género con ese nombre.");
        }
    }
}

public class DistribuidorasController(CineContext db) : CrudController<Distribuidora>(db)
{
    protected override IQueryable<Distribuidora> Listado() =>
        Db.Distribuidoras.Where(d => d.Activo).OrderBy(d => d.RazonSocial);

    protected override string Nombre(Distribuidora distribuidora) => distribuidora.RazonSocial;
}

public class ActoresController(CineContext db) : CrudController<Actor>(db)
{
    protected override IQueryable<Actor> Listado() =>
        Db.Actores.Where(a => a.Activo).OrderBy(a => a.Apellido).ThenBy(a => a.Nombre);

    protected override string Nombre(Actor actor) => actor.NombreCompleto;
}

public class DirectoresController(CineContext db) : CrudController<Director>(db)
{
    protected override IQueryable<Director> Listado() =>
        Db.Directores.Where(d => d.Activo).OrderBy(d => d.Apellido).ThenBy(d => d.Nombre);

    protected override string Nombre(Director director) => director.NombreCompleto;
}

public class SedesController(CineContext db) : CrudController<Sede>(db)
{
    protected override IQueryable<Sede> Listado() =>
        Db.Sedes.Where(s => s.Activo).Include(s => s.Salas.Where(sala => sala.Activo)).OrderBy(s => s.Nombre);

    protected override string Nombre(Sede sede) => sede.Nombre;
}

public class SalasController(CineContext db) : CrudController<Sala>(db)
{
    protected override IQueryable<Sala> Listado() =>
        Db.Salas.Where(s => s.Activo && s.Sede!.Activo).Include(s => s.Sede)
            .OrderBy(s => s.Sede!.Nombre).ThenBy(s => s.Nombre);

    protected override string Nombre(Sala sala) => sala.Nombre;

    protected override async Task CargarOpcionesAsync() =>
        ViewBag.Sedes = new SelectList(
            await Db.Sedes.Where(s => s.Activo).OrderBy(s => s.Nombre).ToListAsync(),
            nameof(Sede.Id), nameof(Sede.Nombre));

    protected override async Task ValidarAsync(Sala sala)
    {
        if (!await Db.Sedes.AnyAsync(s => s.Id == sala.SedeId && s.Activo))
        {
            ModelState.AddModelError(nameof(Sala.SedeId), "Elegí una sede activa.");
        }
    }
}

public class UsuariosController(CineContext db) : CrudController<Usuario>(db)
{
    protected override IQueryable<Usuario> Listado() =>
        Db.Usuarios.OrderBy(u => u.Apellido).ThenBy(u => u.Nombre);

    protected override string Nombre(Usuario usuario) => usuario.NombreCompleto;
}
