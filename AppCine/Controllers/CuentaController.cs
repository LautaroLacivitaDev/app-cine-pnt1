using System.Security.Claims;
using AppCine.Data;
using AppCine.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Controllers;

/// <summary>Login simulado: se elige un usuario de la lista, sin contraseña.</summary>
public class CuentaController(CineContext db) : Controller
{
    public async Task<IActionResult> Ingresar(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(await db.Usuarios.AsNoTracking().OrderBy(u => u.Apellido).ThenBy(u => u.Nombre).ToListAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ingresar(int usuarioId, string? returnUrl)
    {
        var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == usuarioId);
        if (usuario is null)
        {
            return RedirectToAction(nameof(Ingresar));
        }

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreCompleto)
        ];
        if (usuario.EsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, Roles.Admin));
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
            new AuthenticationProperties { IsPersistent = true });

        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult SinPermiso() => View();
}
