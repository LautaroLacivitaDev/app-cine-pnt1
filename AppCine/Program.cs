using System.Globalization;
using AppCine.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Las validaciones se declaran explícitas con Data Annotations, con mensajes en español.
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    mensajes.SetMissingBindRequiredValueAccessor(_ => "Este campo es obligatorio.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, _) => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"{campo} tiene que ser un número.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
});

builder.Services.AddDbContext<CineContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("Cine"))
    .UseSeeding((db, _) => DatosDeEjemplo.GenerarFunciones(db)));

// Login simulado: se elige un usuario de la lista y una cookie recuerda quién es.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Ingresar";
        options.AccessDeniedPath = "/Cuenta/SinPermiso";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Español de Argentina para fechas y moneda, pero con punto decimal para que los inputs numéricos
// del navegador (que siempre envían "1500.50") se lean bien.
var cultura = (CultureInfo)CultureInfo.GetCultureInfo("es-AR").Clone();
cultura.NumberFormat.NumberDecimalSeparator = ".";
cultura.NumberFormat.NumberGroupSeparator = ",";
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = [cultura],
    SupportedUICultures = [cultura]
});

if (app.Environment.IsDevelopment())
{
    // Crea o actualiza la base y genera las funciones de ejemplo al arrancar.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<CineContext>().Database.Migrate();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "admin",
        pattern: "{area:exists}/{controller=Inicio}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
