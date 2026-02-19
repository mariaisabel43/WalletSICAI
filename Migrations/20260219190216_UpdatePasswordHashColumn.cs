using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletSICAI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePasswordHashColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instituciones",
                columns: table => new
                {
                    InstitucionID = table.Column<int>(type: "int", nullable: false),
                    InstitucionNombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CedulaJuridica = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Instituc__706D41E993323C46", x => x.InstitucionID);
                });

            migrationBuilder.CreateTable(
                name: "Administrativos",
                columns: table => new
                {
                    AdministrativoID = table.Column<int>(type: "int", nullable: false),
                    AdministrativoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdministrativoApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdministrativoCedula = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: true),
                    AministrativoPuesto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AdministrativoPassword = table.Column<byte[]>(type: "varbinary(50)", maxLength: 50, nullable: true),
                    AdministrativoSalt = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: true),
                    InstitucionID = table.Column<int>(type: "int", nullable: true),
                    AdministrativoNombreCompleto = table.Column<string>(type: "nvarchar(201)", maxLength: 201, nullable: false, computedColumnSql: "(([AdministrativoNombre]+' ')+[AdministrativoApellido])", stored: false),
                    AdministrativoEmail = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__320E52920CDCF077", x => x.AdministrativoID);
                    table.ForeignKey(
                        name: "FK_Administrativo_Instititucion",
                        column: x => x.InstitucionID,
                        principalTable: "Instituciones",
                        principalColumn: "InstitucionID");
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    EstudianteID = table.Column<int>(type: "int", nullable: false),
                    EstudianteNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstudianteApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstudianteCedula = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false),
                    EstudianteSeccion = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: true),
                    EstudianteFechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    EstudianteNumeroTelefonico = table.Column<string>(type: "varchar(9)", unicode: false, maxLength: 9, nullable: true),
                    EstudianteEmailPersonal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    EstudianteEmailInstitucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    MontoActual = table.Column<int>(type: "int", nullable: true),
                    InstitucionID = table.Column<int>(type: "int", nullable: true),
                    EstudianteNombreCompleto = table.Column<string>(type: "nvarchar(201)", maxLength: 201, nullable: false, computedColumnSql: "(([EstudianteNombre]+' ')+[EstudianteApellido])", stored: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Estudian__6F76833800058BCC", x => x.EstudianteID);
                    table.ForeignKey(
                        name: "FK_Estudiantes_Institucion",
                        column: x => x.InstitucionID,
                        principalTable: "Instituciones",
                        principalColumn: "InstitucionID");
                });

            migrationBuilder.CreateTable(
                name: "Recargas",
                columns: table => new
                {
                    RecargaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteID = table.Column<int>(type: "int", nullable: true),
                    AdministrativoID = table.Column<int>(type: "int", nullable: true),
                    SolicitanteRecargaNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SolicitanteRecargaApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SolicitanteRecargaCedula = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: true),
                    SolicitanteRecargaEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ModoPagoRecarga = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    MontoRecarga = table.Column<int>(type: "int", nullable: true),
                    FechaRecarga = table.Column<DateOnly>(type: "date", nullable: true),
                    SolicitanteRecargaNombreCompleto = table.Column<string>(type: "nvarchar(201)", maxLength: 201, nullable: true, computedColumnSql: "(([SolicitanteRecargaNombre]+' ')+[SolicitanteRecargaApellido])", stored: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Recargas__018A5903E1D95803", x => x.RecargaID);
                    table.ForeignKey(
                        name: "FK_Recarga_Administrativo",
                        column: x => x.AdministrativoID,
                        principalTable: "Administrativos",
                        principalColumn: "AdministrativoID");
                    table.ForeignKey(
                        name: "FK_Recarga_Estudiante",
                        column: x => x.EstudianteID,
                        principalTable: "Estudiantes",
                        principalColumn: "EstudianteID");
                });

            migrationBuilder.CreateTable(
                name: "HistorialRecargaEstudiante",
                columns: table => new
                {
                    HistorialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteID = table.Column<int>(type: "int", nullable: true),
                    RecargaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Historia__975206EF61545E3A", x => x.HistorialID);
                    table.ForeignKey(
                        name: "FK_Historial_Estudiante",
                        column: x => x.EstudianteID,
                        principalTable: "Estudiantes",
                        principalColumn: "EstudianteID");
                    table.ForeignKey(
                        name: "FK_Historial_Recarga",
                        column: x => x.RecargaID,
                        principalTable: "Recargas",
                        principalColumn: "RecargaID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrativos_InstitucionID",
                table: "Administrativos",
                column: "InstitucionID");

            migrationBuilder.CreateIndex(
                name: "UQ__Administ__91C417C77C0FF3F9",
                table: "Administrativos",
                column: "AdministrativoCedula",
                unique: true,
                filter: "[AdministrativoCedula] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_InstitucionID",
                table: "Estudiantes",
                column: "InstitucionID");

            migrationBuilder.CreateIndex(
                name: "UQ__Estudian__E3012E1D4932A87F",
                table: "Estudiantes",
                column: "EstudianteCedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialRecargaEstudiante_EstudianteID",
                table: "HistorialRecargaEstudiante",
                column: "EstudianteID");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialRecargaEstudiante_RecargaID",
                table: "HistorialRecargaEstudiante",
                column: "RecargaID");

            migrationBuilder.CreateIndex(
                name: "UQ__Instituc__024C516541E4417A",
                table: "Instituciones",
                column: "CedulaJuridica",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recargas_AdministrativoID",
                table: "Recargas",
                column: "AdministrativoID");

            migrationBuilder.CreateIndex(
                name: "IX_Recargas_EstudianteID",
                table: "Recargas",
                column: "EstudianteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialRecargaEstudiante");

            migrationBuilder.DropTable(
                name: "Recargas");

            migrationBuilder.DropTable(
                name: "Administrativos");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "Instituciones");
        }
    }
}
