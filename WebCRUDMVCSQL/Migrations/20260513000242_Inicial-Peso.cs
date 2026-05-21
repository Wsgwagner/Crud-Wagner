using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCRUDMVCSQL.Migrations
{
    public partial class InicialPeso : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Peso",
                table: "Produto",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Preco",
                table: "Produto",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Produto");

            migrationBuilder.DropColumn(
                name: "Preco",
                table: "Produto");
        }
    }
}
