using System.ComponentModel.DataAnnotations;

namespace WEBODY.Models
{
    public class Antrenor
    {
        [Key]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [Display(Name = "Uzmanlık Alanı")]
        public string UzmanlikAlani { get; set; } // Örn: Fitness, Pilates

        // --- ÇALIŞMA SAATLERİ ---

        [Display(Name = "Mesai Başlangıç Saati (0-23)")]
        [Range(0, 23, ErrorMessage = "Saat 0 ile 23 arasında olmalıdır.")]
        public int BaslangicSaati { get; set; } = 9; // Varsayılan sabah 09:00

        [Display(Name = "Mesai Bitiş Saati (0-23)")]
        [Range(0, 23, ErrorMessage = "Saat 0 ile 23 arasında olmalıdır.")]
        public int BitisSaati { get; set; } = 17; // Varsayılan akşam 17:00

        // İlişkiler
        public List<Randevu>? Randevular { get; set; }
    }
}