// İlgili model sınıflarını kullanabilmek için namespace import edilir
using EmlakOtomasyonu.Models;

namespace EmlakOtomasyonu.Models.ViewModels
{
    public class IlanVeIcOzellikViewModel
    {
        public IlanTablosu Ilan { get; set; } = new IlanTablosu();
        public IcOzellikTablosu IcOzellik { get; set; } = new IcOzellikTablosu();
        public DisOzellikTablosu DisOzellik { get; set; } = new DisOzellikTablosu();
        public IlanDetayTablosu IlanDetay { get; set; } = new IlanDetayTablosu();
        public List<ResimTablosu> Resimler { get; set; } = new List<ResimTablosu>();
        public IFormFile? VitrinResim { get; set; } // Vitrin resmi dosya yükleme
    }
}