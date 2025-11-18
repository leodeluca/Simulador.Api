using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Simulador.Api.Migrations
{
    /// <inheritdoc />
    public partial class atualizando_entidades4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Perfis_PerfilId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Pontuacao",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "DataSimulacao",
                table: "Simulacoes",
                newName: "Data");

            migrationBuilder.AlterColumn<int>(
                name: "PerfilId",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PontuacaoRisco",
                table: "Clientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Perfis_PerfilId",
                table: "Clientes",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Perfis_PerfilId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "PontuacaoRisco",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Simulacoes",
                newName: "DataSimulacao");

            migrationBuilder.AlterColumn<int>(
                name: "PerfilId",
                table: "Clientes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "Pontuacao",
                table: "Clientes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Perfis_PerfilId",
                table: "Clientes",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id");
        }
    }
}
