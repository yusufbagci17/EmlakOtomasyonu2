// Model katmanında yer alan Admin sınıfı tanımlanıyor
namespace EmlakOtomasyonu.Models
{
    // sealed: Bu sınıfın başka sınıflar tarafından kalıtılamayacağını belirtir (miras alınamaz)
    public sealed class Admin
    {
        // Adminin veritabanındaki benzersiz ID'si
        public int AdminID { get; set; }

        // Adminin adı (kullanıcı adı olarak da kullanılabilir)
        public string AdminAd { get; set; }

        // Adminin soyadı
        public string AdminSoyad { get; set; }

        // Şifreyi dışarıdan doğrudan erişimden korumak için private alan
        private string _adminSifre;

        // Şifreye erişmek ve değiştirmek için kullanılan property
        public string AdminSifre
        {
            get => _adminSifre;          // şifreyi getirme işlemi
            set => _adminSifre = value;  // şifreyi set etme işlemi
        }

        // Boş (parametresiz) constructor: bir nesne örneği oluşturmak için kullanılabilir
        public Admin() { }

        // Parametreli constructor: doğrudan adminin bilgileriyle birlikte nesne oluşturmayı sağlar
        public Admin(string ad, string soyad, string sifre)
        {
            AdminAd = ad;            // ad parametresi sınıfın AdminAd özelliğine atanır
            AdminSoyad = soyad;      // soyad parametresi sınıfın AdminSoyad özelliğine atanır
            _adminSifre = sifre;     // şifre doğrudan private alana atanır
        }

        // Giriş yapma işlemi: parametre olarak verilen kullanıcı adı ve şifre kontrol edilir
        public bool GirisYap(string kullaniciAdi, string sifre)
        {
            // Giriş bilgileri doğruysa true döner, yanlışsa false
            return AdminAd == kullaniciAdi && _adminSifre == sifre;
        }
    }
}
