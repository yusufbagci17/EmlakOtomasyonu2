namespace EmlakOtomasyonu.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; } // Hata oluþtuðunda, istemciye ait benzersiz istek kimliði.

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Eðer RequestId boþ deðilse, hata sayfasýnda gösterilebilir.
    }
}
