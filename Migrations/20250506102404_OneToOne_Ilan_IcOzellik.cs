using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmlakOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class OneToOne_Ilan_IcOzellik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ic_Ozellik_Tablosu_Ilan_Tablosu_IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu");

            migrationBuilder.DropIndex(
                name: "IX_Ic_Ozellik_Tablosu_IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu");

            migrationBuilder.DropColumn(
                name: "IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu");

            migrationBuilder.AlterColumn<string>(
                name: "ilanVitrin",
                table: "Ilan_Tablosu",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ilanVResim",
                table: "Ilan_Tablosu",
                type: "nchar(250)",
                fixedLength: true,
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(250)",
                oldFixedLength: true,
                oldMaxLength: 250);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ilanVitrin",
                table: "Ilan_Tablosu",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ilanVResim",
                table: "Ilan_Tablosu",
                type: "nchar(250)",
                fixedLength: true,
                maxLength: 250,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nchar(250)",
                oldFixedLength: true,
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ic_Ozellik_Tablosu_IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu",
                column: "IlanTablosuIlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ic_Ozellik_Tablosu_Ilan_Tablosu_IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu",
                column: "IlanTablosuIlanId",
                principalTable: "Ilan_Tablosu",
                principalColumn: "ilanID");
        }
    }
}
