namespace EmlakOtomasyonu.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; } // Hata olu�tu�unda, istemciye ait benzersiz istek kimli�i.

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // E�er RequestId bo� de�ilse, hata sayfas�nda g�sterilebilir.
    }
}
