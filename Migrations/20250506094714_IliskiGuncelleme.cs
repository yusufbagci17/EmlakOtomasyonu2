using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmlakOtomasyonu.Migrations
{
    /// <inheritdoc />
    public partial class IliskiGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin_Tablosu",
                columns: table => new
                {
                    adminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    adminAd = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: true),
                    adminSoyad = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: true),
                    AdminKullaniciAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    adminSifre = table.Column<string>(type: "nchar(30)", fixedLength: true, maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin_Tablosu", x => x.adminID);
                });

            migrationBuilder.CreateTable(
                name: "Islem_Tablosu",
                columns: table => new
                {
                    ilanIslemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    islemAd = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islem_Tablosu", x => x.ilanIslemID);
                });

            migrationBuilder.CreateTable(
                name: "Kategori_Tablosu",
                columns: table => new
                {
                    ilanKategoriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kategoriAd = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategori_Tablosu", x => x.ilanKategoriID);
                });

            migrationBuilder.CreateTable(
                name: "Ilan_Tablosu",
                columns: table => new
                {
                    ilanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ilanBaslık = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    ilanFiyat = table.Column<int>(type: "int", nullable: true),
                    ilanTarih = table.Column<DateTime>(type: "datetime", nullable: true),
                    ilanKategoriID = table.Column<int>(type: "int", nullable: false),
                    ilanİslemID = table.Column<int>(type: "int", nullable: false),
                    ilanVitrin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ilanVResim = table.Column<string>(type: "nchar(250)", fixedLength: true, maxLength: 250, nullable: false),
                    ilanAçıklama = table.Column<string>(type: "nchar(1000)", fixedLength: true, maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilan_Tablosu", x => x.ilanID);
                    table.ForeignKey(
                        name: "FK_Ilan_Tablosu_Islem_Tablosu",
                        column: x => x.ilanİslemID,
                        principalTable: "Islem_Tablosu",
                        principalColumn: "ilanIslemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ilan_Tablosu_Kategori_Tablosu",
                        column: x => x.ilanKategoriID,
                        principalTable: "Kategori_Tablosu",
                        principalColumn: "ilanKategoriID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dis_Ozellik_Tablosu",
                columns: table => new
                {
                    doID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    doOtopark = table.Column<bool>(type: "bit", nullable: false),
                    doOyunParkı = table.Column<bool>(type: "bit", nullable: false),
                    doGuvenlik = table.Column<bool>(type: "bit", nullable: false),
                    doKapici = table.Column<bool>(type: "bit", nullable: false),
                    ilanID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dis_Ozellik_Tablosu", x => x.doID);
                    table.ForeignKey(
                        name: "FK_Dis_Ozellik_Tablosu_Ilan_Tablosu",
                        column: x => x.ilanID,
                        principalTable: "Ilan_Tablosu",
                        principalColumn: "ilanID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ic_Ozellik_Tablosu",
                columns: table => new
                {
                    ioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ioAsansor = table.Column<bool>(type: "bit", nullable: false),
                    ioSomine = table.Column<bool>(type: "bit", nullable: false),
                    ioMobilyaTakımı = table.Column<bool>(type: "bit", nullable: false),
                    ioDusKabini = table.Column<bool>(type: "bit", nullable: false),
                    ilanID = table.Column<int>(type: "int", nullable: false),
                    IlanTablosuIlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ic_Ozellik_Tablosu", x => x.ioID);
                    table.ForeignKey(
                        name: "FK_Ic_Ozellik_Tablosu_Ilan_Tablosu",
                        column: x => x.ilanID,
                        principalTable: "Ilan_Tablosu",
                        principalColumn: "ilanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ic_Ozellik_Tablosu_Ilan_Tablosu_IlanTablosuIlanId",
                        column: x => x.IlanTablosuIlanId,
                        principalTable: "Ilan_Tablosu",
                        principalColumn: "ilanID");
                });

            migrationBuilder.CreateTable(
                name: "Ilan_Detay_Tablosu",
                columns: table => new
                {
                    ilanDetayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idOdaSayisi = table.Column<int>(type: "int", nullable: true),
                    idSalonSayisi = table.Column<int>(type: "int", nullable: true),
                    idBinaYasi = table.Column<int>(type: "int", nullable: true),
                    idBinaKatSayisi = table.Column<int>(type: "int", nullable: true),
                    idBinaKacinciKat = table.Column<int>(type: "int", nullable: true),
                    idBinaIsıtma = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    idEsyaliMi = table.Column<bool>(type: "bit", nullable: true),
                    ilanID = table.Column<int>(type: "int", nullable: false),
                    IlanTablosuIlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilan_Detay_Tablosu", x => x.ilanDetayID);
                    table.ForeignKey(
                        name: "FK_Ilan_Detay_Tablosu_Ilan_Tablosu",
                        column: x => x.ilanID,
                        principalTable: "Ilan_Tablosu",
                        principalColumn: "ilanID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ilan_Detay_Tablosu_Ilan_Tablosu_IlanTablosuIlanId",
                        column: x => x.IlanTablosuIlanId,
                        principalTable: "Ilan_Tablosu",
                        principalColumn: "ilanID");
                });

            migrationBuilder.CreateTable(
                name: "Resim_Tablosu",
                columns: table => new
                {
                    resimID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    resimAd = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    resimResim = table.Column<string>(type: "nchar(500)", fixedLength: true, maxLength: 500, nullable: true),
                    ilanID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resim_Tablosu", x => x.resimID);
                    table.ForeignKey(
                        name: "FK_Resim_Tablosu_Ilan_Tablosu",
                        column: x => x.ilanID,
                        principalTable: "Ilan_Tablosu",
                        principalColumn: "ilanID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dis_Ozellik_Tablosu_ilanID",
                table: "Dis_Ozellik_Tablosu",
                column: "ilanID");

            migrationBuilder.CreateIndex(
                name: "IX_Ic_Ozellik_Tablosu_IlanTablosuIlanId",
                table: "Ic_Ozellik_Tablosu",
                column: "IlanTablosuIlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Ic_Ozellik_Tablosu_ilanID",
                table: "Ic_Ozellik_Tablosu",
                column: "ilanID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ilan_Detay_Tablosu_IlanTablosuIlanId",
                table: "Ilan_Detay_Tablosu",
                column: "IlanTablosuIlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Ilan_Detay_Tablosu_ilanID",
                table: "Ilan_Detay_Tablosu",
                column: "ilanID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ilan_Tablosu_ilanİslemID",
                table: "Ilan_Tablosu",
                column: "ilanİslemID");

            migrationBuilder.CreateIndex(
                name: "IX_Ilan_Tablosu_ilanKategoriID",
                table: "Ilan_Tablosu",
                column: "ilanKategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_Resim_Tablosu_ilanID",
                table: "Resim_Tablosu",
                column: "ilanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin_Tablosu");

            migrationBuilder.DropTable(
                name: "Dis_Ozellik_Tablosu");

            migrationBuilder.DropTable(
                name: "Ic_Ozellik_Tablosu");

            migrationBuilder.DropTable(
                name: "Ilan_Detay_Tablosu");

            migrationBuilder.DropTable(
                name: "Resim_Tablosu");

            migrationBuilder.DropTable(
                name: "Ilan_Tablosu");

            migrationBuilder.DropTable(
                name: "Islem_Tablosu");

            migrationBuilder.DropTable(
                name: "Kategori_Tablosu");
        }
    }
}
