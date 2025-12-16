using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBODY.Migrations
{
    /// <inheritdoc />
    public partial class AntrenorSaatleriEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdSoyad",
                table: "Antrenorler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "BaslangicSaati",
                table: "Antrenorler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BitisSaati",
                table: "Antrenorler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaslangicSaati",
                table: "Antrenorler");

            migrationBuilder.DropColumn(
                name: "BitisSaati",
                table: "Antrenorler");

            migrationBuilder.AlterColumn<string>(
                name: "AdSoyad",
                table: "Antrenorler",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
