using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmlakOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dis_Ozellik_Tablosu_ilanID",
                table: "Dis_Ozellik_Tablosu");

            migrationBuilder.AlterColumn<string>(
                name: "resimResim",
                table: "Resim_Tablosu",
                type: "nvarchar(500)",
                fixedLength: true,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(500)",
                oldFixedLength: true,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "resimAd",
                table: "Resim_Tablosu",
                type: "nvarchar(250)",
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(100)",
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dis_Ozellik_Tablosu_ilanID",
                table: "Dis_Ozellik_Tablosu",
                column: "ilanID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dis_Ozellik_Tablosu_ilanID",
                table: "Dis_Ozellik_Tablosu");

            migrationBuilder.AlterColumn<string>(
                name: "resimResim",
                table: "Resim_Tablosu",
                type: "nchar(500)",
                fixedLength: true,
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldFixedLength: true,
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "resimAd",
                table: "Resim_Tablosu",
                type: "nchar(100)",
                fixedLength: true,
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldFixedLength: true,
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dis_Ozellik_Tablosu_ilanID",
                table: "Dis_Ozellik_Tablosu",
                column: "ilanID");
        }
    }
}
