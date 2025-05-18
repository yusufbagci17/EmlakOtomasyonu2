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
            // Ýlk 4 vitrinde gösterilecek ilanlarý alýyoruz.
            var vitrinIlanlari = _context.IlanTablosus
                .Where(i => i.IlanVitrin == true && !string.IsNullOrEmpty(i.IlanVresim))
                .OrderByDescending(i => i.IlanTarih)
                .Take(4)
                .ToList();

            // Vitrin haricinde kalan son eklenen ilanlarý alýyoruz.
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

            ViewBag.Hata = "Kullanýcý adý veya þifre hatalý.";
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
            // Ýlaný veritabanýndan getir
            var ilan = _context.IlanTablosus
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .Include(i => i.IcOzellik)
                .Include(i => i.DisOzellik)
                .Include(i => i.IlanDetay)
                .Include(i => i.ResimTablosus) // Tüm resimleri dahil et
                .FirstOrDefault(i => i.IlanId == ilanId);

            if (ilan == null)
            {
                // Ýlan bulunamazsa anasayfaya yönlendir
                TempData["Hata"] = "Ýlan bulunamadý.";
                return RedirectToAction("Index");
            }

            return View("IlanDetay",ilan);
        }
        public IActionResult SatilikIlanlar()
        {
            // Satýlýk ilanlarý filtrele
            var satilikIlanlar = _context.IlanTablosus
                .Where(i => i.IlanIslem.IslemAd == "Satýlýk") // Ýþlem türü "Satýlýk" olanlarý filtrele
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .ToList();

            return View(satilikIlanlar); // SatilikIlanlar.cshtml görünümüne yönlendir
        }
        public IActionResult KiralikIlanlar()
        {
            // Kiralýk ilanlarý filtrele
            var kiralikIlanlar = _context.IlanTablosus
                .Where(i => i.IlanIslem.IslemAd == "Kiralýk") // Ýþlem türü "Kiralýk" olanlarý filtrele
                .Include(i => i.IlanKategori)
                .Include(i => i.IlanIslem)
                .ToList();

            return View(kiralikIlanlar); // KiralikIlanlar.cshtml görünümüne yönlendir
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
                ModelState.AddModelError("", "Lütfen eksik bilgileri doldurun.");
                return View(model);
            }

            try
            {
                var ilan = model.Ilan;
                ilan.IlanTarih = DateTime.Now;

                // Kullanýcýnýn "Vitrine ekle" seçeneðini iþaretleyip iþaretlemediðini kontrol et
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

                    ilan.IlanVresim = fileName; // Resim dosya adý veritabanýna kaydedilecek
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
                    ModelState.AddModelError("", "Lütfen en az 3, en fazla 10 resim yükleyin.");
                    return View(model);
                }

                // Baðlantýlý tablolar
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
                TempData["Basari"] = "Ýlan ve resimler baþarýyla eklendi!";
                return RedirectToAction("AdminPanel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ýlan eklenirken bir hata oluþtu.");
                ModelState.AddModelError("", "Ýlan eklenirken bir hata oluþtu. Lütfen tekrar deneyiniz.");
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
                // Ýlaný ve iliþkili verileri getir
                var ilan = await _context.IlanTablosus
                    .Include(i => i.IcOzellik)
                    .Include(i => i.DisOzellik)
                    .Include(i => i.IlanDetay)
                    .Include(i => i.ResimTablosus) // Resimleri dahil ettik
                    .FirstOrDefaultAsync(i => i.IlanId == ilanId);

                if (ilan == null)
                {
                    TempData["Hata"] = "Silinmek istenen ilan bulunamadý.";
                    return RedirectToAction("AdminPanel");
                }

                // Ýlanýn iliþkili resimlerini sil
                if (ilan.ResimTablosus.Any())
                {
                    foreach (var resim in ilan.ResimTablosus)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", resim.ResimResim);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath); // Fiziksel dosyayý sil
                        }
                        _context.ResimTablosus.Remove(resim); // Veritabanýndan sil
                    }
                }

                // Ýlanýn iliþkili verilerini sil
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

                // Ýlaný sil
                _context.IlanTablosus.Remove(ilan);

                // Veritabaný deðiþikliklerini kaydet
                await _context.SaveChangesAsync();
                TempData["Basari"] = "Ýlan ve resimleri baþarýyla silindi!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ýlan silme iþlemi sýrasýnda bir hata oluþtu. Ýlan ID: {ilanId}");
                TempData["Hata"] = "Ýlan silinirken bir hata oluþtu. Lütfen tekrar deneyiniz.";
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
                TempData["Hata"] = "Düzenlenmek istenen ilan bulunamadý.";
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
                TempData["Hata"] = "Güncellenmek istenen ilan bulunamadý.";
                return RedirectToAction("AdminPanel");
            }

            try
            {
                // Ýlan bilgilerini güncelle
                ilan.IlanBaslik = model.Ilan.IlanBaslik;
                ilan.IlanFiyat = model.Ilan.IlanFiyat;
                ilan.IlanAciklama = model.Ilan.IlanAciklama;
                ilan.IlanKategoriId = model.Ilan.IlanKategoriId;
                ilan.IlanIslemId = model.Ilan.IlanIslemId;
                ilan.IlanVitrin = model.Ilan.IlanVitrin;

                // Vitrin Resim Güncelleme
                if (VitrinResim != null && VitrinResim.Length > 0)
                {
                    var fileName = Path.GetFileName(VitrinResim.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await VitrinResim.CopyToAsync(stream);
                    }

                    ilan.IlanVresim = fileName; // Yeni resim dosya adý veritabanýna kaydedilecek
                }

                // Silinecek Resimleri Ýþleme
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
                                System.IO.File.Delete(filePath); // Fiziksel dosyayý sil
                            }

                            _context.ResimTablosus.Remove(silinecekResim); // Veritabanýndan sil
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

                // Ýç Özellikleri Güncelle
                if (ilan.IcOzellik != null)
                {
                    ilan.IcOzellik.IoAsansor = model.IcOzellik.IoAsansor;
                    ilan.IcOzellik.IoSomine = model.IcOzellik.IoSomine;
                    ilan.IcOzellik.IoMobilyaTakimi = model.IcOzellik.IoMobilyaTakimi;
                    ilan.IcOzellik.IoDusKabini = model.IcOzellik.IoDusKabini;
                }

                // Dýþ Özellikleri Güncelle
                if (ilan.DisOzellik != null)
                {
                    ilan.DisOzellik.DoOtopark = model.DisOzellik.DoOtopark;
                    ilan.DisOzellik.DoOyunParký = model.DisOzellik.DoOyunParký;
                    ilan.DisOzellik.DoGuvenlik = model.DisOzellik.DoGuvenlik;
                    ilan.DisOzellik.DoKapici = model.DisOzellik.DoKapici;
                }

                // Ýlan Detaylarýný Güncelle
                if (ilan.IlanDetay != null)
                {
                    ilan.IlanDetay.IdOdaSayisi = model.IlanDetay.IdOdaSayisi;
                    ilan.IlanDetay.IdSalonSayisi = model.IlanDetay.IdSalonSayisi;
                    ilan.IlanDetay.IdBinaYasi = model.IlanDetay.IdBinaYasi;
                    ilan.IlanDetay.IdBinaKatSayisi = model.IlanDetay.IdBinaKatSayisi;
                    ilan.IlanDetay.IdBinaKacinciKat = model.IlanDetay.IdBinaKacinciKat;
                    ilan.IlanDetay.IdBinaIsýtma = model.IlanDetay.IdBinaIsýtma;
                    ilan.IlanDetay.IdEsyaliMi = model.IlanDetay.IdEsyaliMi;
                }

                await _context.SaveChangesAsync();
                TempData["Basari"] = "Ýlan baþarýyla güncellendi!";
                return RedirectToAction("AdminPanel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ýlan güncelleme sýrasýnda bir hata oluþtu.");
                TempData["Hata"] = "Ýlan güncellenirken bir hata oluþtu. Lütfen tekrar deneyiniz.";
                return View(model);
            }
        }
    }
}