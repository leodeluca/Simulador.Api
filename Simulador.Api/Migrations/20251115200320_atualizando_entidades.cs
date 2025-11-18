using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulador.Api.Migrations
{
    /// <inheritdoc />
    public partial class atualizando_entidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "Simuladores",
                newName: "ValorInvestido");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Simuladores",
                newName: "ValorFinal");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Simuladores",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Simuladores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataSimulacao",
                table: "Simuladores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrazoMeses",
                table: "Simuladores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "Simuladores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Simuladores_ClienteId",
                table: "Simuladores",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Simuladores_ProdutoId",
                table: "Simuladores",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Simuladores_Clientes_ClienteId",
                table: "Simuladores",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Simuladores_Produtos_ProdutoId",
                table: "Simuladores",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Simuladores_Clientes_ClienteId",
                table: "Simuladores");

            migrationBuilder.DropForeignKey(
                name: "FK_Simuladores_Produtos_ProdutoId",
                table: "Simuladores");

            migrationBuilder.DropIndex(
                name: "IX_Simuladores_ClienteId",
                table: "Simuladores");

            migrationBuilder.DropIndex(
                name: "IX_Simuladores_ProdutoId",
                table: "Simuladores");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Simuladores");

            migrationBuilder.DropColumn(
                name: "DataSimulacao",
                table: "Simuladores");

            migrationBuilder.DropColumn(
                name: "PrazoMeses",
                table: "Simuladores");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "Simuladores");

            migrationBuilder.RenameColumn(
                name: "ValorInvestido",
                table: "Simuladores",
                newName: "Valor");

            migrationBuilder.RenameColumn(
                name: "ValorFinal",
                table: "Simuladores",
                newName: "Nome");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Simuladores",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
