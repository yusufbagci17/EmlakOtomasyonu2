// Models/KategoriTablosu.cs
using System;
using System.Collections.Generic;

namespace EmlakOtomasyonu.Models
{
    public partial class KategoriTablosu // 'KategoriTablosu' sınıfı, ilanların kategorilerini temsil eder.
    {
        public int? IlanKategoriId { get; set; } // 'IlanKategoriId', kategoriyi tanımlayan benzersiz kimliktir. Nullable bir alandır çünkü kategori her zaman girilmeyebilir.

        public string? KategoriAd { get; set; } // 'KategoriAd', kategori adını belirtir. Nullable bir alandır çünkü kategori adı her zaman girilmeyebilir.

        public virtual ICollection<IlanTablosu> IlanTablosus { get; set; } = new List<IlanTablosu>(); // 'IlanTablosus', bu kategoriyle ilişkilendirilmiş ilanların koleksiyonudur. 
        // Yani, bu koleksiyon, ilgili kategoriye ait ilanları tutar. 'IlanTablosu' tablosuyla olan ilişkidir.
    }
}
