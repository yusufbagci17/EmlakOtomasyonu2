using EmlakOtomasyonu.Models; // 'EmlakOtomasyonu.Models' namespace'ini içeri aktarıyor. Bu, 'IlanTablosu' gibi diğer model sınıflarına erişim sağlar.
using System.ComponentModel.DataAnnotations.Schema; // Veritabanı ile ilişkileri tanımlayan attribute'lar için gerekli namespace.

public class IcOzellikTablosu // 'IcOzellikTablosu' sınıfı, iç özellikleri temsil eden tabloyu ifade eder.
{
    public int IoID { get; set; } // 'IoID' iç özellik tablosunun benzersiz birincil anahtarıdır. Her kaydın farklı bir ID'si olacaktır.

    public bool IoAsansor { get; set; } // 'IoAsansor', iç özelliklerden biri olup, dairenin asansöre sahip olup olmadığını belirtir. Boolean (true/false) değeri alır.
    public bool IoSomine { get; set; } // 'IoSomine', evde şömine olup olmadığını belirtir. Boolean (true/false) değeri alır.
    public bool IoMobilyaTakimi { get; set; } // 'IoMobilyaTakimi', evin mobilyalı olup olmadığını belirtir. Boolean (true/false) değeri alır.
    public bool IoDusKabini { get; set; } // 'IoDusKabini', duş kabini olup olmadığını belirtir. Boolean (true/false) değeri alır.

    public int IlanID { get; set; } // 'IlanID', bu iç özelliklerin hangi ilana ait olduğunu belirtir. Bu, dış anahtar (foreign key) olarak kullanılır.

    [ForeignKey("IlanID")] // Bu attribute, 'IlanID' özelliğinin bir dış anahtar (foreign key) olduğunu belirtir. 'IlanTablosu' tablosundaki 'IlanID' ile ilişkilidir.
    public virtual IlanTablosu? Ilan { get; set; } // 'Ilan', 'IlanID' ile ilişkilendirilen ve 'IlanTablosu' sınıfına işaret eden sanal bir özelliktir.
    // Bu ilişki, her iç özelliğin bir ilana bağlı olmasını sağlar ve 'IlanTablosu' ile bağlantıyı kurar.
}
