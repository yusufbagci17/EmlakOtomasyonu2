using System.ComponentModel.DataAnnotations.Schema;

namespace EmlakOtomasyonu.Models
{
    public partial class IlanTablosu
    {
        public int IlanId { get; set; }
        public string? IlanBaslik { get; set; }
        public int? IlanFiyat { get; set; }
        public DateTime? IlanTarih { get; set; }

        public int? IlanKategoriId { get; set; }

        [ForeignKey("IlanKategoriId")]
        public KategoriTablosu? IlanKategori { get; set; }

        public int IlanIslemId { get; set; }

        [ForeignKey("IlanIslemId")]
        public IslemTablosu? IlanIslem { get; set; }
        public bool IlanVitrin { get; set; }
        public string? IlanVresim { get; set; }
        public string? IlanAciklama { get; set; }

        public virtual IlanDetayTablosu? IlanDetay { get; set; }
        public virtual IcOzellikTablosu? IcOzellik { get; set; }
        public virtual DisOzellikTablosu? DisOzellik { get; set; }
        public virtual ICollection<ResimTablosu> ResimTablosus { get; set; } = new List<ResimTablosu>();
    }
}
