// Models/Ilan.cs
namespace EmlakOtomasyonu.Models // 'EmlakOtomasyonu.Models' namespace'ini içeri aktarıyor. Bu sınıf, emlak otomasyonu uygulamasındaki ilanları temsil eder.
{
    public class Ilan // 'Ilan' sınıfı, ilan ile ilgili tüm bilgileri içeren bir model sınıfıdır.
    {
        public int IlanID { get; set; } // 'IlanID', her bir ilanın benzersiz kimliğini temsil eder. Veritabanındaki birincil anahtar (primary key).

        public string? IlanBaslik { get; set; } // 'IlanBaslik', ilanın başlığını tutar. Örneğin, "Satılık 3+1 Daire". Nullable (null olabilen) bir alandır.

        public decimal IlanFiyat { get; set; } // 'IlanFiyat', ilanın fiyatını belirtir. 'decimal' türü, parasal değerleri saklamak için uygundur.

        public DateTime IlanTarih { get; set; } // 'IlanTarih', ilanın veritabanına eklendiği tarihi belirtir. Bu, ilanın kaydedildiği zamanı izlemek için kullanılır.

        public int IlanKategoriID { get; set; } // 'IlanKategoriID', ilanı belirli bir kategoriye atar. Bu, kategorilerin ID'lerini saklar (örneğin, "Ev", "İş Yeri").

        // 'IlanIslemID
        public int IlanIslemID { get; set; }
    }
}
