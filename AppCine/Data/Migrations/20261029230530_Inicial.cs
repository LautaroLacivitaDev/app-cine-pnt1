using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppCine.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Distribuidoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distribuidoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Generos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Localidad = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Genero = table.Column<int>(type: "int", nullable: false),
                    EsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Peliculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Clasificacion = table.Column<int>(type: "int", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrailerUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaEstreno = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    DirectorId = table.Column<int>(type: "int", nullable: false),
                    DistribuidoraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peliculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Peliculas_Distribuidoras_DistribuidoraId",
                        column: x => x.DistribuidoraId,
                        principalTable: "Distribuidoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Peliculas_Personas_DirectorId",
                        column: x => x.DirectorId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Salas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CantidadFilas = table.Column<int>(type: "int", nullable: false),
                    ButacasPorFila = table.Column<int>(type: "int", nullable: false),
                    Soporta3D = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salas_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeliculaGenero",
                columns: table => new
                {
                    GeneroId = table.Column<int>(type: "int", nullable: false),
                    PeliculaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeliculaGenero", x => new { x.GeneroId, x.PeliculaId });
                    table.ForeignKey(
                        name: "FK_PeliculaGenero_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeliculaGenero_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repartos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeliculaId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<int>(type: "int", nullable: false),
                    Personaje = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repartos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repartos_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Repartos_Personas_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Funciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Formato = table.Column<int>(type: "int", nullable: false),
                    Idioma = table.Column<int>(type: "int", nullable: false),
                    Subtitulada = table.Column<bool>(type: "bit", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    PeliculaId = table.Column<int>(type: "int", nullable: false),
                    SalaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Funciones_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Funciones_Salas_SalaId",
                        column: x => x.SalaId,
                        principalTable: "Salas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Distribuidoras",
                columns: new[] { "Id", "Activo", "RazonSocial" },
                values: new object[,]
                {
                    { 1, true, "United International Pictures S.R.L." },
                    { 2, true, "Distribuidora Andes S.A." },
                    { 3, true, "Cine Sur Films S.R.L." }
                });

            migrationBuilder.InsertData(
                table: "Generos",
                columns: new[] { "Id", "Activo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Drama" },
                    { 2, true, "Comedia" },
                    { 3, true, "Suspenso" },
                    { 4, true, "Animación" },
                    { 5, true, "Ciencia ficción" }
                });

            migrationBuilder.InsertData(
                table: "Personas",
                columns: new[] { "Id", "Activo", "Apellido", "Discriminator", "Nacionalidad", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Campanella", "Director", "Argentina", "Juan José" },
                    { 2, true, "Szifron", "Director", "Argentina", "Damián" },
                    { 3, true, "Bielinsky", "Director", "Argentina", "Fabián" },
                    { 4, true, "Mitre", "Director", "Argentina", "Santiago" },
                    { 5, true, "Lasseter", "Director", "Estados Unidos", "John" },
                    { 6, true, "Nolan", "Director", "Reino Unido", "Christopher" },
                    { 10, true, "Darín", "Actor", "Argentina", "Ricardo" },
                    { 11, true, "Villamil", "Actor", "Argentina", "Soledad" },
                    { 12, true, "Rivas", "Actor", "Argentina", "Érica" },
                    { 13, true, "Pauls", "Actor", "Argentina", "Gastón" },
                    { 14, true, "Lanzani", "Actor", "Argentina", "Peter" },
                    { 15, true, "Hanks", "Actor", "Estados Unidos", "Tom" },
                    { 16, true, "Allen", "Actor", "Estados Unidos", "Tim" },
                    { 17, true, "McConaughey", "Actor", "Estados Unidos", "Matthew" },
                    { 18, true, "Hathaway", "Actor", "Estados Unidos", "Anne" }
                });

            migrationBuilder.InsertData(
                table: "Sedes",
                columns: new[] { "Id", "Activo", "Direccion", "Localidad", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Av. Corrientes 1500", "CABA", "Sede Centro" },
                    { 2, true, "Av. Maipú 2200", "Vicente López", "Sede Norte" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Apellido", "Email", "EsAdmin", "FechaNacimiento", "Genero", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "Gómez", "laura.gomez@example.com", true, new DateTime(1990, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Laura", "11 5555-0001" },
                    { 2, "Pérez", "martin.perez@example.com", false, new DateTime(1985, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, "Martín", "11 5555-0002" },
                    { 3, "Díaz", "sofia.diaz@example.com", false, new DateTime(2011, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Sofía", "11 5555-0003" }
                });

            migrationBuilder.InsertData(
                table: "Peliculas",
                columns: new[] { "Id", "Activo", "Clasificacion", "Descripcion", "DirectorId", "DistribuidoraId", "DuracionMinutos", "FechaEstreno", "ImagenUrl", "Titulo", "TrailerUrl" },
                values: new object[,]
                {
                    { 1, true, 2, "Un empleado judicial retirado escribe una novela sobre un caso de asesinato que nunca pudo cerrar.", 1, 1, 129, new DateTime(2009, 8, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/400x600/1f2937/f9fafb/png?text=El%20secreto%20de%20sus%20ojos", "El secreto de sus ojos", "https://www.youtube.com/results?search_query=El%20secreto%20de%20sus%20ojos%20trailer" },
                    { 2, true, 2, "Seis historias independientes de personas que pierden el control frente a la injusticia cotidiana.", 2, 2, 122, new DateTime(2014, 8, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/400x600/1f2937/f9fafb/png?text=Relatos%20salvajes", "Relatos salvajes", "https://www.youtube.com/results?search_query=Relatos%20salvajes%20trailer" },
                    { 3, true, 1, "Dos estafadores se cruzan por casualidad y arman en un día el golpe de sus vidas.", 3, 3, 114, new DateTime(2000, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/400x600/1f2937/f9fafb/png?text=Nueve%20reinas", "Nueve reinas", "https://www.youtube.com/results?search_query=Nueve%20reinas%20trailer" },
                    { 4, true, 1, "Los fiscales Strassera y Moreno Ocampo llevan adelante el Juicio a las Juntas.", 4, 1, 140, new DateTime(2022, 9, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/400x600/1f2937/f9fafb/png?text=Argentina%2C%201985", "Argentina, 1985", "https://www.youtube.com/results?search_query=Argentina%2C%201985%20trailer" },
                    { 5, true, 0, "Los juguetes de Andy cobran vida cuando nadie los ve, y la llegada de Buzz lo cambia todo.", 5, 2, 81, new DateTime(1995, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/400x600/1f2937/f9fafb/png?text=Toy%20Story", "Toy Story", "https://www.youtube.com/results?search_query=Toy%20Story%20trailer" },
                    { 6, true, 1, "Un grupo de exploradores viaja a través de un agujero de gusano en busca de un nuevo hogar.", 6, 1, 169, new DateTime(2014, 11, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://placehold.co/400x600/1f2937/f9fafb/png?text=Interestelar", "Interestelar", "https://www.youtube.com/results?search_query=Interestelar%20trailer" }
                });

            migrationBuilder.InsertData(
                table: "Salas",
                columns: new[] { "Id", "Activo", "ButacasPorFila", "CantidadFilas", "Nombre", "SedeId", "Soporta3D" },
                values: new object[,]
                {
                    { 1, true, 20, 12, "Sala 1", 1, true },
                    { 2, true, 16, 10, "Sala 2", 1, false },
                    { 3, true, 14, 8, "Sala 3", 1, false },
                    { 4, true, 22, 14, "Sala 1", 2, true },
                    { 5, true, 18, 10, "Sala 2", 2, false },
                    { 6, true, 15, 9, "Sala 3", 2, false }
                });

            migrationBuilder.InsertData(
                table: "PeliculaGenero",
                columns: new[] { "GeneroId", "PeliculaId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 6 },
                    { 2, 2 },
                    { 2, 5 },
                    { 3, 1 },
                    { 3, 3 },
                    { 4, 5 },
                    { 5, 6 }
                });

            migrationBuilder.InsertData(
                table: "Repartos",
                columns: new[] { "Id", "ActorId", "PeliculaId", "Personaje" },
                values: new object[,]
                {
                    { 1, 10, 1, "Benjamín Espósito" },
                    { 2, 11, 1, "Irene Menéndez Hastings" },
                    { 3, 10, 2, "Simón Fischer" },
                    { 4, 12, 2, "Romina" },
                    { 5, 10, 3, "Marcos" },
                    { 6, 13, 3, "Juan" },
                    { 7, 10, 4, "Julio César Strassera" },
                    { 8, 14, 4, "Luis Moreno Ocampo" },
                    { 9, 15, 5, "Woody" },
                    { 10, 16, 5, "Buzz Lightyear" },
                    { 11, 17, 6, "Cooper" },
                    { 12, 18, 6, "Amelia Brand" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funciones_FechaHora",
                table: "Funciones",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_Funciones_PeliculaId",
                table: "Funciones",
                column: "PeliculaId");

            migrationBuilder.CreateIndex(
                name: "IX_Funciones_SalaId_FechaHora",
                table: "Funciones",
                columns: new[] { "SalaId", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Generos_Nombre",
                table: "Generos",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeliculaGenero_PeliculaId",
                table: "PeliculaGenero",
                column: "PeliculaId");

            migrationBuilder.CreateIndex(
                name: "IX_Peliculas_DirectorId",
                table: "Peliculas",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Peliculas_DistribuidoraId",
                table: "Peliculas",
                column: "DistribuidoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Repartos_ActorId",
                table: "Repartos",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Repartos_PeliculaId_ActorId",
                table: "Repartos",
                columns: new[] { "PeliculaId", "ActorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Salas_SedeId",
                table: "Salas",
                column: "SedeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Funciones");

            migrationBuilder.DropTable(
                name: "PeliculaGenero");

            migrationBuilder.DropTable(
                name: "Repartos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Salas");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "Peliculas");

            migrationBuilder.DropTable(
                name: "Sedes");

            migrationBuilder.DropTable(
                name: "Distribuidoras");

            migrationBuilder.DropTable(
                name: "Personas");
        }
    }
}
