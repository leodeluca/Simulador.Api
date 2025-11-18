using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulador.Api.Migrations
{
    /// <inheritdoc />
    public partial class atualizando_entidades5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investimentos_Clientes_ClienteId",
                table: "Investimentos");

            migrationBuilder.DropTable(
                name: "Simuladores");

            migrationBuilder.DropIndex(
                name: "IX_Investimentos_ClienteId",
                table: "Investimentos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Investimentos",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Investimentos",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rentabilidade",
                table: "Investimentos",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Investimentos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Investimentos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Investimentos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Tipo",
                table: "Investimentos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rentabilidade",
                table: "Investimentos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Data",
                table: "Investimentos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "ClienteId",
                table: "Investimentos",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "Simuladores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    ProdutoId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataSimulacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PrazoMeses = table.Column<int>(type: "INTEGER", nullable: true),
                    ValorFinal = table.Column<decimal>(type: "TEXT", nullable: true),
                    ValorInvestido = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simuladores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simuladores_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Simuladores_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investimentos_ClienteId",
                table: "Investimentos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Simuladores_ClienteId",
                table: "Simuladores",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Simuladores_ProdutoId",
                table: "Simuladores",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Investimentos_Clientes_ClienteId",
                table: "Investimentos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
