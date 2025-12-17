using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBODY.Migrations
{
    /// <inheritdoc />
    public partial class RandevuUcretEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HizmetAdi",
                table: "Randevular",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Ucret",
                table: "Randevular",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HizmetAdi",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "Ucret",
                table: "Randevular");
        }
    }
}
