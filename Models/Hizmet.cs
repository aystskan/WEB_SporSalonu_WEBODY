using System.ComponentModel.DataAnnotations;

namespace WebProjeAyseT.Models
{
    public class Hizmet
    {
        [Key]
        public int HizmetId { get; set; }

        [Display(Name = "Hizmet Adı")]
        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        public string HizmetAdi { get; set; } // Örn: Özel Ders, Grup Dersi

        [Display(Name = "Süre (Dakika)")]
        
        [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")] // Aralık kontrolü [cite: 1993]
        public int Sure { get; set; }

        [Display(Name = "Ücret")]
        [DataType(DataType.Currency)]
        public decimal Ucret { get; set; }

        // İlişki: Bir hizmet birçok randevuda kullanılabilir
        public virtual ICollection<Randevu> Randevular { get; set; }
    }
}
