// Gerekli sistem kütüphaneleri dahil edilir
using System;
using System.Collections.Generic;

// Sınıf EmlakOtomasyonu.Models isim alanında yer alır
namespace EmlakOtomasyonu.Models;

// partial ifadesi: Sınıfın başka bir dosyada da tamamlanabileceğini belirtir
public partial class AdminTablosu
{
    // Admin'in veritabanındaki benzersiz kimliği (primary key)
    public int AdminId { get; set; }

    // Admin'in adı - nullable olarak tanımlanmış (veri boş olabilir)
    public string? AdminAd { get; set; }

    // Admin'in soyadı - nullable
    public string? AdminSoyad { get; set; }

    // Admin'in kullanıcı adı - nullable
    public string? AdminKullaniciAdi { get; set; }

    // Admin'in şifresi - nullable
    public string? AdminSifre { get; set; }
}
