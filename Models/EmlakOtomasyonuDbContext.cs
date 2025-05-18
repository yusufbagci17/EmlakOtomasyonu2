// Entity Framework Core kullanımı için gerekli kütüphaneler dahil ediliyor
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EmlakOtomasyonu.Models;

// DbContext sınıfı, veritabanıyla olan etkileşimi yönetir
public partial class EmlakOtomasyonuDbContext : DbContext
{
    // Parametresiz constructor (kullanılmıyor ancak genellikle Entity Framework için gereklidir)
    public EmlakOtomasyonuDbContext() { }

    // DbContextOptions ile başlatma
    public EmlakOtomasyonuDbContext(DbContextOptions<EmlakOtomasyonuDbContext> options)
        : base(options)
    { }

    // Tabloları DbSet olarak tanımlıyoruz. Bu, Entity Framework'ün veritabanı ile etkileşimde kullanacağı sınıflardır.
    public virtual DbSet<AdminTablosu> AdminTablosus { get; set; }
    public virtual DbSet<DisOzellikTablosu> DisOzellikTablosus { get; set; }
    public virtual DbSet<IcOzellikTablosu> IcOzellikTablosus { get; set; }
    public virtual DbSet<IlanDetayTablosu> IlanDetayTablosus { get; set; }
    public virtual DbSet<IlanTablosu> IlanTablosus { get; set; }
    public virtual DbSet<IslemTablosu> IslemTablosus { get; set; }
    public virtual DbSet<KategoriTablosu> KategoriTablosus { get; set; }
    public virtual DbSet<ResimTablosu> ResimTablosus { get; set; }

    // Veritabanı bağlantısını yapılandırmak için bu metod kullanılır
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // SQL Server bağlantı dizesi belirtiliyor
        // Uygulama veritabanı ile bağlantıyı bu dize ile gerçekleştirir
        optionsBuilder.UseSqlServer("Server=USER\\SQLEXPRESS;Database=EmlakOtomasyonuDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    // Veritabanı modelini konfigüre etme, ilişkiler ve tablo yapıları burada belirlenir
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AdminTablosu modelinin yapılandırılması
        modelBuilder.Entity<AdminTablosu>(entity =>
        {
            entity.HasKey(e => e.AdminId); // AdminId birincil anahtar
            entity.ToTable("Admin_Tablosu"); // Tablo adı
            entity.Property(e => e.AdminId).HasColumnName("adminID");
            entity.Property(e => e.AdminAd).HasMaxLength(30).IsFixedLength().HasColumnName("adminAd");
            entity.Property(e => e.AdminSoyad).HasMaxLength(30).IsFixedLength().HasColumnName("adminSoyad");
            entity.Property(e => e.AdminKullaniciAdi).HasMaxLength(30).IsFixedLength().HasColumnName("adminKullaniciAdi");
            entity.Property(e => e.AdminSifre).HasMaxLength(30).IsFixedLength().HasColumnName("adminSifre");
        });

        // DisOzellikTablosu modelinin yapılandırılması
        modelBuilder.Entity<DisOzellikTablosu>(entity =>
        {
            entity.HasKey(e => e.DoId); // DoId birincil anahtar
            entity.ToTable("Dis_Ozellik_Tablosu"); // Tablo adı
            entity.Property(e => e.DoId).HasColumnName("doID");
            entity.Property(e => e.DoGuvenlik).HasColumnName("doGuvenlik");
            entity.Property(e => e.DoKapici).HasColumnName("doKapici");
            entity.Property(e => e.DoOtopark).HasColumnName("doOtopark");
            entity.Property(e => e.DoOyunParkı).HasColumnName("doOyunParkı");
            entity.Property(e => e.IlanId).HasColumnName("ilanID");

            // İlana dış özellik eklenmesi ilişkisi
            entity.HasOne(d => d.Ilan)
                  .WithOne(p => p.DisOzellik)
                  .HasForeignKey<DisOzellikTablosu>(d => d.IlanId)
                  .HasConstraintName("FK_Dis_Ozellik_Tablosu_Ilan_Tablosu");
        });

        // IcOzellikTablosu modelinin yapılandırılması
        modelBuilder.Entity<IcOzellikTablosu>(entity =>
        {
            entity.HasKey(e => e.IoID); // IoID birincil anahtar
            entity.ToTable("Ic_Ozellik_Tablosu"); // Tablo adı
            entity.Property(e => e.IoID).HasColumnName("ioID");
            entity.Property(e => e.IlanID).HasColumnName("ilanID");
            entity.Property(e => e.IoAsansor).HasColumnName("ioAsansor");
            entity.Property(e => e.IoDusKabini).HasColumnName("ioDusKabini");
            entity.Property(e => e.IoMobilyaTakimi).HasColumnName("ioMobilyaTakımı");
            entity.Property(e => e.IoSomine).HasColumnName("ioSomine");

            // İlana iç özellik eklenmesi ilişkisi
            entity.HasOne(d => d.Ilan)
                  .WithOne(p => p.IcOzellik)
                  .HasForeignKey<IcOzellikTablosu>(d => d.IlanID)
                  .HasConstraintName("FK_Ic_Ozellik_Tablosu_Ilan_Tablosu");
        });

        // IlanDetayTablosu modelinin yapılandırılması
        modelBuilder.Entity<IlanDetayTablosu>(entity =>
        {
            entity.HasKey(e => e.IlanDetayID); // IlanDetayId birincil anahtar
            entity.ToTable("Ilan_Detay_Tablosu"); // Tablo adı
            entity.Property(e => e.IlanDetayID).HasColumnName("ilanDetayID");
            entity.Property(e => e.IdBinaIsıtma).HasMaxLength(50).IsFixedLength().HasColumnName("idBinaIsıtma");
            entity.Property(e => e.IdBinaKacinciKat).HasColumnName("idBinaKacinciKat");
            entity.Property(e => e.IdBinaKatSayisi).HasColumnName("idBinaKatSayisi");
            entity.Property(e => e.IdBinaYasi).HasColumnName("idBinaYasi");
            entity.Property(e => e.IdEsyaliMi).HasColumnName("idEsyaliMi");
            entity.Property(e => e.IdOdaSayisi).HasColumnName("idOdaSayisi");
            entity.Property(e => e.IdSalonSayisi).HasColumnName("idSalonSayisi");
            entity.Property(e => e.IlanId).HasColumnName("ilanID");

            // IlanDetayTablosu ile IlanTablosu arasındaki ilişki
            modelBuilder.Entity<IlanTablosu>()
                .HasOne(i => i.IlanDetay)
                .WithOne(i => i.Ilan)
                .HasForeignKey<IlanDetayTablosu>(i => i.IlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // IlanTablosu modelinin yapılandırılması
        modelBuilder.Entity<IlanTablosu>(entity =>
        {
            entity.HasKey(e => e.IlanId); // IlanId birincil anahtar
            entity.ToTable("Ilan_Tablosu"); // Tablo adı
            entity.Property(e => e.IlanId).HasColumnName("ilanID");
            entity.Property(e => e.IlanAciklama).HasMaxLength(1000).IsFixedLength().HasColumnName("ilanAçıklama");
            entity.Property(e => e.IlanBaslik).HasMaxLength(100).IsFixedLength().HasColumnName("ilanBaslık");
            entity.Property(e => e.IlanFiyat).HasColumnName("ilanFiyat");
            entity.Property(e => e.IlanKategoriId).HasColumnName("ilanKategoriID");
            entity.Property(e => e.IlanTarih).HasColumnType("datetime").HasColumnName("ilanTarih");
            entity.Property(e => e.IlanVitrin).HasColumnName("ilanVitrin");
            entity.Property(e => e.IlanVresim).HasMaxLength(250).IsFixedLength().HasColumnName("ilanVResim");
            entity.Property(e => e.IlanIslemId).HasColumnName("ilanİslemID");

            // İlanTablosu ile KategoriTablosu arasındaki ilişki
            entity.HasOne(d => d.IlanKategori)
                  .WithMany(p => p.IlanTablosus)
                  .HasForeignKey(d => d.IlanKategoriId)
                  .HasConstraintName("FK_Ilan_Tablosu_Kategori_Tablosu");

            // İlanTablosu ile IslemTablosu arasındaki ilişki
            entity.HasOne(d => d.IlanIslem)
                  .WithMany(p => p.IlanTablosus)
                  .HasForeignKey(d => d.IlanIslemId)
                  .HasConstraintName("FK_Ilan_Tablosu_Islem_Tablosu");
        });

        // IslemTablosu modelinin yapılandırılması
        modelBuilder.Entity<IslemTablosu>(entity =>
        {
            entity.HasKey(e => e.IlanIslemId); // IlanIslemId birincil anahtar
            entity.ToTable("Islem_Tablosu"); // Tablo adı
            entity.Property(e => e.IlanIslemId).HasColumnName("ilanIslemID");
            entity.Property(e => e.IslemAd).HasMaxLength(50).IsFixedLength().HasColumnName("islemAd");
        });

        // KategoriTablosu modelinin yapılandırılması
        modelBuilder.Entity<KategoriTablosu>(entity =>
        {
            entity.HasKey(e => e.IlanKategoriId); // IlanKategoriId birincil anahtar
            entity.ToTable("Kategori_Tablosu"); // Tablo adı
            entity.Property(e => e.IlanKategoriId).HasColumnName("ilanKategoriID");
            entity.Property(e => e.KategoriAd).HasMaxLength(50).IsFixedLength().HasColumnName("kategoriAd");
        });

        // ResimTablosu modelinin yapılandırılması
        modelBuilder.Entity<ResimTablosu>(entity =>
        {
            entity.HasKey(e => e.ResimId); // ResimId birincil anahtar
            entity.ToTable("Resim_Tablosu"); // Tablo adı
            entity.Property(e => e.ResimId).HasColumnName("resimID");
            entity.Property(e => e.IlanId).HasColumnName("ilanID");
            entity.Property(e => e.ResimAd).HasMaxLength(100).IsFixedLength().HasColumnName("resimAd");
            entity.Property(e => e.ResimResim).HasMaxLength(500).IsFixedLength().HasColumnName("resimResim");

            // ResimTablosu ile IlanTablosu arasındaki ilişki
            entity.HasOne(d => d.Ilan)
                  .WithMany(p => p.ResimTablosus)
                  .HasForeignKey(d => d.IlanId)
                  .HasConstraintName("FK_Resim_Tablosu_Ilan_Tablosu");
        });

        // Ek özel yapılandırmalar varsa burada yapılabilir
        OnModelCreatingPartial(modelBuilder);
    }
    // Ek özel yapılandırmalar varsa burada yapılabilir
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}