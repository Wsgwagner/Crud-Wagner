using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCRUDMVCSQL.Migrations
{
    public partial class Inicialsenha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Usuários",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Usuários");
        }
    }
}
