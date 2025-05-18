// Models/ResimTablosu.cs
using System.ComponentModel.DataAnnotations.Schema;

namespace EmlakOtomasyonu.Models
{
    // 'ResimTablosu' sınıfı ilanlara ait resim bilgilerini tutmak için kullanılan bir sınıftır.
    public partial class ResimTablosu
    {
        // 'ResimId' her resmin benzersiz kimliğini (ID) tutar.
        public int ResimId { get; set; }

        // 'ResimAd' resmin adını tutar. Bu, kullanıcıların resme anlamlı bir isim vermesini sağlar.
        [Column(TypeName = "nvarchar(250)")]
        public string? ResimAd { get; set; }

        // 'ResimResim' ise resmin veritabanında saklanacak olan yolunu ya da resmin URL'sini tutar.
        // Bu, resmin saklandığı dosya yolunun ya da URL'nin string formatındaki halidir.
        [Column(TypeName = "nvarchar(500)")]
        public string? ResimResim { get; set; }

        // 'IlanId' bu resmin hangi ilana ait olduğunu belirler. İlanlarla ilişkili her resim bir 'IlanId' taşır.
        public int? IlanId { get; set; }

        // 'Ilan' özelliği, bu resmin hangi ilana ait olduğunu gösteren bir ilişkiyi tanımlar.
        // Bu ilişki, Entity Framework tarafından foreign key ilişkisi olarak işlenir.
        public virtual IlanTablosu? Ilan { get; set; }
    }
}
