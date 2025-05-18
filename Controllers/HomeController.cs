using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using EmlakOtomasyonu.Models;
using EmlakOtomasyonu.Models.ViewModels;
using EmlakOtomasyonu.Data;

namespace EmlakOtomasyonu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // �lk 4 vitrinde g�sterilecek ilanlar� al�yoruz.
            var vitrinIlanlari = _context.IlanTablosus
                .Where(i => i.IlanVitrin == true && !string.IsNullOrEmpty(i.IlanVresim))
                .OrderByDescending(i => i.IlanTarih)
                .Take(4)
                .ToList();

            // Vitrin haricinde kalan son eklenen ilanlar� al�yoruz.
            var sonEklenenler = _context.IlanTablosus
                .Where(i => i.IlanVitrin == false && !string.IsNullOrEmpty(i.IlanVresim))
                .OrderByDescending(i => i.IlanTarih)
                .ToList();

            ViewBag.VitrinIlanlari = vitrinIlanlari;
            ViewBag.SonEklenenler = sonEklenenler;

            return View();
        }


        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string kullaniciAdi, string sifre)
        {
            var admin = _context.AdminTablosus
                .FirstOrDefault(a => a.AdminKullaniciAdi == kullaniciAdi && a.AdminSifre == sifre);

            if (admin != null)
            {
                HttpContext.Session.SetString("Giris", "true");
                return RedirectToAction("AdminPanel");
            }

            ViewBag.Hata = "Kullan�c� ad� veya �ifre hatal�.";
            return View();
        }

        public IActionResult AdminPanel()
        {
            if (HttpContext.Session.GetString("Giris") != "true")
                return RedirectToAction("Login");

            var ilanlar = _context.IlanTablosus
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .Include(i => i.IcOzellik)
                .Include(i => i.DisOzellik)
                .Include(i => i.IlanDetay)
                .Include(i => i.ResimTablosus) // Resimler dahil edildi
                .ToList();

            return View(ilanlar);
        }
        public IActionResult IlanDetay(int ilanId)
        {
            // �lan� veritaban�ndan getir
            var ilan = _context.IlanTablosus
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .Include(i => i.IcOzellik)
                .Include(i => i.DisOzellik)
                .Include(i => i.IlanDetay)
                .Include(i => i.ResimTablosus) // T�m resimleri dahil et
                .FirstOrDefault(i => i.IlanId == ilanId);

            if (ilan == null)
            {
                // �lan bulunamazsa anasayfaya y�nlendir
                TempData["Hata"] = "�lan bulunamad�.";
                return RedirectToAction("Index");
            }

            return View("IlanDetay",ilan);
        }
        public IActionResult SatilikIlanlar()
        {
            // Sat�l�k ilanlar� filtrele
            var satilikIlanlar = _context.IlanTablosus
                .Where(i => i.IlanIslem.IslemAd == "Sat�l�k") // ��lem t�r� "Sat�l�k" olanlar� filtrele
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .ToList();

            return View(satilikIlanlar); // SatilikIlanlar.cshtml g�r�n�m�ne y�nlendir
        }
        public IActionResult KiralikIlanlar()
        {
            // Kiral�k ilanlar� filtrele
            var kiralikIlanlar = _context.IlanTablosus
                .Where(i => i.IlanIslem.IslemAd == "Kiral�k") // ��lem t�r� "Kiral�k" olanlar� filtrele
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .ToList();

            return View(kiralikIlanlar); // KiralikIlanlar.cshtml g�r�n�m�ne y�nlendir
        }
        [HttpPost]
        [HttpGet]
        public IActionResult IlanEkle()
        {
            if (HttpContext.Session.GetString("Giris") != "true")
                return RedirectToAction("Login");

            ViewBag.Kategoriler = new SelectList(_context.KategoriTablosus, "IlanKategoriId", "KategoriAd");
            ViewBag.Islemler = new SelectList(_context.IslemTablosus, "IlanIslemId", "IslemAd");

            return View(new IlanVeIcOzellikViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> IlanEkle(IlanVeIcOzellikViewModel model, IFormFile VitrinResim, List<IFormFile> Resimler)
        {
            if (HttpContext.Session.GetString("Giris") != "true")
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                ViewBag.Kategoriler = new SelectList(_context.KategoriTablosus, "IlanKategoriId", "KategoriAd");
                ViewBag.Islemler = new SelectList(_context.IslemTablosus, "IlanIslemId", "IslemAd");
                ModelState.AddModelError("", "L�tfen eksik bilgileri doldurun.");
                return View(model);
            }

            try
            {
                var ilan = model.Ilan;
                ilan.IlanTarih = DateTime.Now;

                // Kullan�c�n�n "Vitrine ekle" se�ene�ini i�aretleyip i�aretlemedi�ini kontrol et
                ilan.IlanVitrin = model.Ilan.IlanVitrin;

                // Vitrin Resmi Kaydetme
                if (VitrinResim != null && VitrinResim.Length > 0)
                {
                    var fileName = Path.GetFileName(VitrinResim.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await VitrinResim.CopyToAsync(stream);
                    }

                    ilan.IlanVresim = fileName; // Resim dosya ad� veritaban�na kaydedilecek
                }

                _context.IlanTablosus.Add(ilan);
                await _context.SaveChangesAsync();

                // Ek Resimlerin Kaydedilmesi
                if (Resimler != null && Resimler.Count >= 3 && Resimler.Count <= 10)
                {
                    foreach (var resim in Resimler)
                    {
                        var fileName = Path.GetFileName(resim.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await resim.CopyToAsync(stream);
                        }

                        var resimTablosu = new ResimTablosu
                        {
                            ResimAd = fileName,
                            ResimResim = fileName,
                            IlanId = ilan.IlanId
                        };
                        _context.ResimTablosus.Add(resimTablosu);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("", "L�tfen en az 3, en fazla 10 resim y�kleyin.");
                    return View(model);
                }

                // Ba�lant�l� tablolar
                if (model.IcOzellik != null)
                {
                    model.IcOzellik.IlanID = ilan.IlanId;
                    _context.IcOzellikTablosus.Add(model.IcOzellik);
                }

                if (model.DisOzellik != null)
                {
                    model.DisOzellik.IlanId = ilan.IlanId;
                    _context.DisOzellikTablosus.Add(model.DisOzellik);
                }

                if (model.IlanDetay != null)
                {
                    model.IlanDetay.IlanId = ilan.IlanId;
                    _context.IlanDetayTablosus.Add(model.IlanDetay);
                }

                await _context.SaveChangesAsync();
                TempData["Basari"] = "�lan ve resimler ba�ar�yla eklendi!";
                return RedirectToAction("AdminPanel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�lan eklenirken bir hata olu�tu.");
                ModelState.AddModelError("", "�lan eklenirken bir hata olu�tu. L�tfen tekrar deneyiniz.");
                ViewBag.Kategoriler = new SelectList(_context.KategoriTablosus, "IlanKategoriId", "KategoriAd");
                ViewBag.Islemler = new SelectList(_context.IslemTablosus, "IlanIslemId", "IslemAd");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> IlanSil(int ilanId)
        {
            if (HttpContext.Session.GetString("Giris") != "true")
                return RedirectToAction("Login");

            try
            {
                // �lan� ve ili�kili verileri getir
                var ilan = await _context.IlanTablosus
                    .Include(i => i.IcOzellik)
                    .Include(i => i.DisOzellik)
                    .Include(i => i.IlanDetay)
                    .Include(i => i.ResimTablosus) // Resimleri dahil ettik
                    .FirstOrDefaultAsync(i => i.IlanId == ilanId);

                if (ilan == null)
                {
                    TempData["Hata"] = "Silinmek istenen ilan bulunamad�.";
                    return RedirectToAction("AdminPanel");
                }

                // �lan�n ili�kili resimlerini sil
                if (ilan.ResimTablosus.Any())
                {
                    foreach (var resim in ilan.ResimTablosus)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", resim.ResimResim);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath); // Fiziksel dosyay� sil
                        }
                        _context.ResimTablosus.Remove(resim); // Veritaban�ndan sil
                    }
                }

                // �lan�n ili�kili verilerini sil
                if (ilan.IcOzellik != null)
                {
                    _context.IcOzellikTablosus.Remove(ilan.IcOzellik);
                }

                if (ilan.DisOzellik != null)
                {
                    _context.DisOzellikTablosus.Remove(ilan.DisOzellik);
                }

                if (ilan.IlanDetay != null)
                {
                    _context.IlanDetayTablosus.Remove(ilan.IlanDetay);
                }

                // �lan� sil
                _context.IlanTablosus.Remove(ilan);

                // Veritaban� de�i�ikliklerini kaydet
                await _context.SaveChangesAsync();
                TempData["Basari"] = "�lan ve resimleri ba�ar�yla silindi!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"�lan silme i�lemi s�ras�nda bir hata olu�tu. �lan ID: {ilanId}");
                TempData["Hata"] = "�lan silinirken bir hata olu�tu. L�tfen tekrar deneyiniz.";
            }

            return RedirectToAction("AdminPanel");
        }

        [HttpGet]
        public IActionResult IlanDuzenle(int ilanId)
        {
            if (HttpContext.Session.GetString("Giris") != "true")
                return RedirectToAction("Login");

            var ilan = _context.IlanTablosus
                .Include(i => i.IcOzellik)
                .Include(i => i.DisOzellik)
                .Include(i => i.IlanDetay)
                .Include(i => i.ResimTablosus) // Resimleri dahil ettik
                .FirstOrDefault(i => i.IlanId == ilanId);

            if (ilan == null)
            {
                TempData["Hata"] = "D�zenlenmek istenen ilan bulunamad�.";
                return RedirectToAction("AdminPanel");
            }

            var viewModel = new IlanVeIcOzellikViewModel
            {
                Ilan = ilan,
                IcOzellik = ilan.IcOzellik ?? new IcOzellikTablosu(),
                DisOzellik = ilan.DisOzellik ?? new DisOzellikTablosu(),
                IlanDetay = ilan.IlanDetay ?? new IlanDetayTablosu()
            };

            ViewBag.Kategoriler = new SelectList(_context.KategoriTablosus, "IlanKategoriId", "KategoriAd", ilan.IlanKategoriId);
            ViewBag.Islemler = new SelectList(_context.IslemTablosus, "IlanIslemId", "IslemAd", ilan.IlanIslemId);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IlanDuzenle(IlanVeIcOzellikViewModel model, IFormFile VitrinResim, List<IFormFile> YeniResimler, List<int> SilinecekResimIdler)
        {
            if (HttpContext.Session.GetString("Giris") != "true")
                return RedirectToAction("Login");

            var ilan = _context.IlanTablosus
                .Include(i => i.IcOzellik)
                .Include(i => i.DisOzellik)
                .Include(i => i.IlanDetay)
                .Include(i => i.ResimTablosus) // Resimler dahil edildi
                .FirstOrDefault(i => i.IlanId == model.Ilan.IlanId);

            if (ilan == null)
            {
                TempData["Hata"] = "G�ncellenmek istenen ilan bulunamad�.";
                return RedirectToAction("AdminPanel");
            }

            try
            {
                // �lan bilgilerini g�ncelle
                ilan.IlanBaslik = model.Ilan.IlanBaslik;
                ilan.IlanFiyat = model.Ilan.IlanFiyat;
                ilan.IlanAciklama = model.Ilan.IlanAciklama;
                ilan.IlanKategoriId = model.Ilan.IlanKategoriId;
                ilan.IlanIslemId = model.Ilan.IlanIslemId;
                ilan.IlanVitrin = model.Ilan.IlanVitrin;

                // Vitrin Resim G�ncelleme
                if (VitrinResim != null && VitrinResim.Length > 0)
                {
                    var fileName = Path.GetFileName(VitrinResim.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await VitrinResim.CopyToAsync(stream);
                    }

                    ilan.IlanVresim = fileName; // Yeni resim dosya ad� veritaban�na kaydedilecek
                }

                // Silinecek Resimleri ��leme
                if (SilinecekResimIdler != null && SilinecekResimIdler.Count > 0)
                {
                    foreach (var resimId in SilinecekResimIdler)
                    {
                        var silinecekResim = ilan.ResimTablosus.FirstOrDefault(r => r.ResimId == resimId);
                        if (silinecekResim != null)
                        {
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", silinecekResim.ResimResim);
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath); // Fiziksel dosyay� sil
                            }

                            _context.ResimTablosus.Remove(silinecekResim); // Veritaban�ndan sil
                        }
                    }
                }

                // Yeni Resimlerin Eklenmesi
                if (YeniResimler != null && YeniResimler.Count > 0)
                {
                    foreach (var resim in YeniResimler)
                    {
                        var fileName = Path.GetFileName(resim.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await resim.CopyToAsync(stream);
                        }

                        var yeniResim = new ResimTablosu
                        {
                            ResimAd = fileName,
                            ResimResim = fileName,
                            IlanId = ilan.IlanId
                        };
                        _context.ResimTablosus.Add(yeniResim);
                    }
                }

                // �� �zellikleri G�ncelle
                if (ilan.IcOzellik != null)
                {
                    ilan.IcOzellik.IoAsansor = model.IcOzellik.IoAsansor;
                    ilan.IcOzellik.IoSomine = model.IcOzellik.IoSomine;
                    ilan.IcOzellik.IoMobilyaTakimi = model.IcOzellik.IoMobilyaTakimi;
                    ilan.IcOzellik.IoDusKabini = model.IcOzellik.IoDusKabini;
                }

                // D�� �zellikleri G�ncelle
                if (ilan.DisOzellik != null)
                {
                    ilan.DisOzellik.DoOtopark = model.DisOzellik.DoOtopark;
                    ilan.DisOzellik.DoOyunPark� = model.DisOzellik.DoOyunPark�;
                    ilan.DisOzellik.DoGuvenlik = model.DisOzellik.DoGuvenlik;
                    ilan.DisOzellik.DoKapici = model.DisOzellik.DoKapici;
                }

                // �lan Detaylar�n� G�ncelle
                if (ilan.IlanDetay != null)
                {
                    ilan.IlanDetay.IdOdaSayisi = model.IlanDetay.IdOdaSayisi;
                    ilan.IlanDetay.IdSalonSayisi = model.IlanDetay.IdSalonSayisi;
                    ilan.IlanDetay.IdBinaYasi = model.IlanDetay.IdBinaYasi;
                    ilan.IlanDetay.IdBinaKatSayisi = model.IlanDetay.IdBinaKatSayisi;
                    ilan.IlanDetay.IdBinaKacinciKat = model.IlanDetay.IdBinaKacinciKat;
                    ilan.IlanDetay.IdBinaIs�tma = model.IlanDetay.IdBinaIs�tma;
                    ilan.IlanDetay.IdEsyaliMi = model.IlanDetay.IdEsyaliMi;
                }

                await _context.SaveChangesAsync();
                TempData["Basari"] = "�lan ba�ar�yla g�ncellendi!";
                return RedirectToAction("AdminPanel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "�lan g�ncelleme s�ras�nda bir hata olu�tu.");
                TempData["Hata"] = "�lan g�ncellenirken bir hata olu�tu. L�tfen tekrar deneyiniz.";
                return View(model);
            }
        }
    }
}