using AppCine.Models;
using Microsoft.EntityFrameworkCore;

namespace AppCine.Data;

public class CineContext(DbContextOptions<CineContext> options) : DbContext(options)
{
    public DbSet<Persona> Personas => Set<Persona>();
    public DbSet<Actor> Actores => Set<Actor>();
    public DbSet<Director> Directores => Set<Director>();
    public DbSet<Reparto> Repartos => Set<Reparto>();
    public DbSet<Pelicula> Peliculas => Set<Pelicula>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<Distribuidora> Distribuidoras => Set<Distribuidora>();
    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<Sala> Salas => Set<Sala>();
    public DbSet<Funcion> Funciones => Set<Funcion>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TPH: Actor y Director comparten la tabla Personas con la columna Discriminator.
        modelBuilder.Entity<Persona>().ToTable("Personas")
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Actor>(nameof(Actor))
            .HasValue<Director>(nameof(Director));

        modelBuilder.Entity<Pelicula>()
            .HasMany(p => p.Generos)
            .WithMany(g => g.Peliculas)
            .UsingEntity("PeliculaGenero",
                r => r.HasOne(typeof(Genero)).WithMany().HasForeignKey("GeneroId"),
                l => l.HasOne(typeof(Pelicula)).WithMany().HasForeignKey("PeliculaId"),
                j => j.HasData(DatosDeEjemplo.PeliculaGeneros));

        modelBuilder.Entity<Reparto>().HasIndex(r => new { r.PeliculaId, r.ActorId }).IsUnique();
        modelBuilder.Entity<Genero>().HasIndex(g => g.Nombre).IsUnique();
        modelBuilder.Entity<Funcion>().HasIndex(f => new { f.SalaId, f.FechaHora });
        modelBuilder.Entity<Funcion>().HasIndex(f => f.FechaHora);
        modelBuilder.Entity<Funcion>().Property(f => f.Precio).HasPrecision(12, 2);

        // Baja lógica: nada se borra en cascada (además SQL Server rechaza los caminos múltiples de cascada).
        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            if (fk.DeclaringEntityType.Name != "PeliculaGenero")
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        DatosDeEjemplo.Cargar(modelBuilder);
    }
}
