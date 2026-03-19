using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WalletSICAI.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministrativoId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Administrativos_AdministrativoId",
                        column: x => x.AdministrativoId,
                        principalTable: "Administrativos",
                        principalColumn: "AdministrativoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_AdministrativoId",
                table: "PasswordResetTokens",
                column: "AdministrativoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetTokens");
        }
    }
}
