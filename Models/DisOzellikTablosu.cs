// Gerekli kütüphaneler dahil ediliyor
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmlakOtomasyonu.Models;

// Bu sınıf veritabanındaki DisOzellikTablosu adlı tabloyu temsil eder
public partial class DisOzellikTablosu
{
    // Bu tablonun birincil anahtarı (Primary Key)
    public int DoId { get; set; }

    // Dış özellikler: Her biri bool türündedir, yani evet/hayır şeklindedir

    // Otopark var mı?
    public bool DoOtopark { get; set; }

    // Oyun parkı var mı?
    public bool DoOyunParkı { get; set; }

    // Güvenlik var mı?
    public bool DoGuvenlik { get; set; }

    // Kapıcı var mı?
    public bool DoKapici { get; set; }

    // Bu dış özelliklerin bağlı olduğu ilan
    public int IlanId { get; set; }

    // IlanId property'si IlanTablosu ile ilişkilidir (Foreign Key)
    [ForeignKey("IlanId")]
    public virtual IlanTablosu? Ilan { get; set; }
    // virtual: Entity Framework’ün Lazy Loading özelliği için kullanılır
}
