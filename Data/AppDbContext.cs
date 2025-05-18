using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using EmlakOtomasyonu.Models;

namespace EmlakOtomasyonu.Data
{
    // Veritabanı ile uygulaman arasında köprü kuran sınıf
    public partial class AppDbContext : DbContext
    {
        // Parametresiz constructor (gerekirse kullanılabilir)
        public AppDbContext() { }

        // DI (dependency injection) ile context oluşturulurken kullanılacak constructor
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Veritabanındaki tabloları temsil eden DbSet'ler
        public virtual DbSet<AdminTablosu> AdminTablosus { get; set; }
        public virtual DbSet<DisOzellikTablosu> DisOzellikTablosus { get; set; }
        public virtual DbSet<IcOzellikTablosu> IcOzellikTablosus { get; set; }
        public virtual DbSet<IlanDetayTablosu> IlanDetayTablosus { get; set; }
        public virtual DbSet<IlanTablosu> IlanTablosus { get; set; }
        public virtual DbSet<IslemTablosu> IslemTablosus { get; set; }
        public virtual DbSet<KategoriTablosu> KategoriTablosus { get; set; }
        public virtual DbSet<ResimTablosu> ResimTablosus { get; set; }

        // Veritabanı bağlantı ayarı
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Server=USER\\SQLEXPRESS;Database=EmlakOtomasyonuDB;Trusted_Connection=True;TrustServerCertificate=True;");

        // Veritabanı tablolarının yapılandırılması ve ilişkilerin kurulması
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Admin tablosu yapılandırması
            modelBuilder.Entity<AdminTablosu>(entity =>
            {
                entity.HasKey(e => e.AdminId);
                entity.ToTable("Admin_Tablosu");
                entity.Property(e => e.AdminId).HasColumnName("adminID");
                entity.Property(e => e.AdminAd).HasMaxLength(30).IsFixedLength().HasColumnName("adminAd");
                entity.Property(e => e.AdminSoyad).HasMaxLength(30).IsFixedLength().HasColumnName("adminSoyad");
                entity.Property(e => e.AdminSifre).HasMaxLength(30).IsFixedLength().HasColumnName("adminSifre");
            });

            // Ilan - IcOzellik bire bir ilişkisi
            modelBuilder.Entity<IlanTablosu>()
                .HasOne(i => i.IcOzellik)
                .WithOne(io => io.Ilan)
                .HasForeignKey<IcOzellikTablosu>(io => io.IlanID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ilan - DisOzellik bire bir ilişkisi
            modelBuilder.Entity<IlanTablosu>()
                .HasOne(i => i.DisOzellik)
                .WithOne(d => d.Ilan)
                .HasForeignKey<DisOzellikTablosu>(d => d.IlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IlanTablosu>()
                .HasOne(i => i.IlanDetay)
                .WithOne(i => i.Ilan)
                .HasForeignKey<IlanDetayTablosu>(i => i.IlanId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
            // Dis Özellik tablosu yapılandırması
            modelBuilder.Entity<DisOzellikTablosu>(entity =>
            {
                entity.HasKey(e => e.DoId);
                entity.ToTable("Dis_Ozellik_Tablosu");
                entity.Property(e => e.DoId).HasColumnName("doID");
                entity.Property(e => e.DoGuvenlik).HasColumnName("doGuvenlik");
                entity.Property(e => e.DoKapici).HasColumnName("doKapici");
                entity.Property(e => e.DoOtopark).HasColumnName("doOtopark");
                entity.Property(e => e.DoOyunParkı).HasColumnName("doOyunParkı");
                entity.Property(e => e.IlanId).HasColumnName("ilanID");
                entity.HasOne(d => d.Ilan)
                    .WithOne(p => p.DisOzellik)
                    .HasForeignKey<DisOzellikTablosu>(d => d.IlanId)
                    .HasConstraintName("FK_Dis_Ozellik_Tablosu_Ilan_Tablosu");
            });

            // İç Özellik tablosu yapılandırması
            modelBuilder.Entity<IcOzellikTablosu>(entity =>
            {
                entity.HasKey(e => e.IoID);
                entity.ToTable("Ic_Ozellik_Tablosu");
                entity.Property(e => e.IoID).HasColumnName("ioID");
                entity.Property(e => e.IlanID).HasColumnName("ilanID");
                entity.Property(e => e.IoAsansor).HasColumnName("ioAsansor");
                entity.Property(e => e.IoDusKabini).HasColumnName("ioDusKabini");
                entity.Property(e => e.IoMobilyaTakimi).HasColumnName("ioMobilyaTakımı");
                entity.Property(e => e.IoSomine).HasColumnName("ioSomine");
                entity.HasOne(d => d.Ilan)
                    .WithOne(p => p.IcOzellik)
                    .HasForeignKey<IcOzellikTablosu>(d => d.IlanID)
                    .HasConstraintName("FK_Ic_Ozellik_Tablosu_Ilan_Tablosu");
            });

            // Ilan Detay tablosu yapılandırması
            modelBuilder.Entity<IlanDetayTablosu>(entity =>
            {
                entity.HasKey(e => e.IlanDetayID);
                entity.ToTable("Ilan_Detay_Tablosu");
                entity.Property(e => e.IlanDetayID).HasColumnName("ilanDetayID");
                entity.Property(e => e.IdBinaIsıtma).HasMaxLength(50).IsFixedLength().HasColumnName("idBinaIsıtma");
                entity.Property(e => e.IdBinaKacinciKat).HasColumnName("idBinaKacinciKat");
                entity.Property(e => e.IdBinaKatSayisi).HasColumnName("idBinaKatSayisi");
                entity.Property(e => e.IdBinaYasi).HasColumnName("idBinaYasi");
                entity.Property(e => e.IdEsyaliMi).HasColumnName("idEsyaliMi");
                entity.Property(e => e.IdOdaSayisi).HasColumnName("idOdaSayisi");
                entity.Property(e => e.IdSalonSayisi).HasColumnName("idSalonSayisi");
                entity.Property(e => e.IlanId).HasColumnName("ilanID");
                entity.HasOne(d => d.Ilan)
                    .WithOne(p => p.IlanDetay)
                    .HasForeignKey<IlanDetayTablosu>(d => d.IlanId)
                    .HasConstraintName("FK_Ilan_Detay_Tablosu_Ilan_Tablosu");
            });

            // Ilan tablosu yapılandırması
            modelBuilder.Entity<IlanTablosu>(entity =>
            {
                entity.HasKey(e => e.IlanId);
                entity.ToTable("Ilan_Tablosu");
                entity.Property(e => e.IlanId).HasColumnName("ilanID");
                entity.Property(e => e.IlanAciklama).HasMaxLength(1000).IsFixedLength().HasColumnName("ilanAçıklama");
                entity.Property(e => e.IlanBaslik).HasMaxLength(100).IsFixedLength().HasColumnName("ilanBaslık");
                entity.Property(e => e.IlanFiyat).HasColumnName("ilanFiyat");
                entity.Property(e => e.IlanKategoriId).HasColumnName("ilanKategoriID");
                entity.Property(e => e.IlanTarih).HasColumnType("datetime").HasColumnName("ilanTarih");
                entity.Property(e => e.IlanVitrin).HasColumnName("ilanVitrin");
                entity.Property(e => e.IlanVresim).HasMaxLength(250).IsFixedLength().HasColumnName("ilanVResim");
                entity.Property(e => e.IlanIslemId).HasColumnName("ilanİslemID");

                entity.HasOne(d => d.IlanKategori)
                    .WithMany(p => p.IlanTablosus)
                    .HasForeignKey(d => d.IlanKategoriId)
                    .HasConstraintName("FK_Ilan_Tablosu_Kategori_Tablosu");

                entity.HasOne(d => d.IlanIslem)
                    .WithMany(p => p.IlanTablosus)
                    .HasForeignKey(d => d.IlanIslemId)
                    .HasConstraintName("FK_Ilan_Tablosu_Islem_Tablosu");
            });

            // İşlem tablosu yapılandırması
            modelBuilder.Entity<IslemTablosu>(entity =>
            {
                entity.HasKey(e => e.IlanIslemId);
                entity.ToTable("Islem_Tablosu");
                entity.Property(e => e.IlanIslemId).HasColumnName("ilanIslemID");
                entity.Property(e => e.IslemAd).HasMaxLength(50).IsFixedLength().HasColumnName("islemAd");
            });

            // Kategori tablosu yapılandırması
            modelBuilder.Entity<KategoriTablosu>(entity =>
            {
                entity.HasKey(e => e.IlanKategoriId);
                entity.ToTable("Kategori_Tablosu");
                entity.Property(e => e.IlanKategoriId).HasColumnName("ilanKategoriID");
                entity.Property(e => e.KategoriAd).HasMaxLength(50).IsFixedLength().HasColumnName("kategoriAd");
            });

            // Resim tablosu yapılandırması
            modelBuilder.Entity<ResimTablosu>(entity =>
            {
                entity.HasKey(e => e.ResimId);
                entity.ToTable("Resim_Tablosu");
                entity.Property(e => e.ResimId).HasColumnName("resimID");
                entity.Property(e => e.IlanId).HasColumnName("ilanID");
                entity.Property(e => e.ResimAd).HasMaxLength(250).IsFixedLength().HasColumnName("resimAd");
                entity.Property(e => e.ResimResim).HasMaxLength(500).IsFixedLength().HasColumnName("resimResim");
                entity.HasOne(d => d.Ilan)
                    .WithMany(p => p.ResimTablosus)
                    .HasForeignKey(d => d.IlanId)
                    .HasConstraintName("FK_Resim_Tablosu_Ilan_Tablosu");
            });

            // Kısmi metod (isteğe bağlı başka konfigürasyonlar için)
            OnModelCreatingPartial(modelBuilder);
        }

        // Kısmi sınıf için boş metod tanımı (başka dosyada override edilebilir)
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
