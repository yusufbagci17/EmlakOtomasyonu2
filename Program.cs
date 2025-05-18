using EmlakOtomasyonu.Data; // NAMESPACE GÜNCELLENDİ
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// Eğer Identity kullanacaksanız (User ve Admin rolleri için daha kapsamlı bir çözüm):
// using Microsoft.AspNetCore.Identity;
// using EmlakOtomasyonu.Models; // User modeliniz IdentityUser'dan türemiyorsa veya özel User modeliniz için

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext'i ekle (ADIM 3'te güncellediğimiz AppDbContext'i kullanacak)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// appsettings.json dosyanızda "DefaultConnection" adında bir bağlantı dizesi olduğundan emin olun.

// Basit Cookie Authentication ayarları (GitHub'daki orijinal dosyanızdan alındı)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "UserLoginCookie"; // Cookie adı
        options.LoginPath = "/Home/Giris";       // Giriş yapılmamışsa yönlendirilecek sayfa
        options.AccessDeniedPath = "/Home/Giris";  // Yetkisiz erişimde yönlendirilecek sayfa
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Cookie geçerlilik süresi (örneğin 60 dakika)
        options.SlidingExpiration = true; // Her istekte süreyi uzat
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // wwwroot klasöründeki statik dosyaların (CSS, JS, resimler) sunulmasını sağlar.

app.UseRouting(); // Yönlendirme mekanizmasını etkinleştirir.

app.UseAuthentication(); // Kimlik doğrulama middleware'ini etkinleştirir. Bu satır UseAuthorization'dan ÖNCE olmalı.
app.UseAuthorization(); // Yetkilendirme middleware'ini etkinleştirir.

// Varsayılan Controller rotası. Yeni eklediğimiz IlanController da bu rota üzerinden çalışacak.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();