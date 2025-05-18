// Models/IslemTablosu.cs
using System;
using System.Collections.Generic;

namespace EmlakOtomasyonu.Models
{
    public partial class IslemTablosu // 'IslemTablosu' sınıfı, işlem türlerini temsil eder (örneğin, Satılık, Kiralık).
    {
        public int IlanIslemId { get; set; } // 'IlanIslemId', işlem türü için benzersiz kimlik (primary key) tanımlar.

        public string? IslemAd { get; set; } // 'IslemAd', işlem türünün adını belirtir. Nullable bir alandır çünkü işlem adı her zaman girilmeyebilir.

        public virtual ICollection<IlanTablosu> IlanTablosus { get; set; } = new List<IlanTablosu>(); // 'IlanTablosus', bu işlem türüyle ilişkilendirilmiş ilanların koleksiyonudur. 
        // Yani, bu koleksiyon, ilgili işlem türüne ait ilanları tutar. 'IlanTablosu' tablosuyla olan ilişkidir.
    }
}
