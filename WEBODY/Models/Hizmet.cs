using System.ComponentModel.DataAnnotations;

namespace WEBODY.Models
{
    public class Hizmet
    {
        [Key]
        public int HizmetId { get; set; }

        [Required(ErrorMessage = "Hizmet adı boş geçilemez.")]
        [Display(Name = "Hizmet Adı")]
        public string Ad { get; set; } // Örn: Fitness Üyeliği, Özel Ders

        [Range(0, 10000, ErrorMessage = "Ücret 0 ile 10.000 TL arasında olmalıdır.")] // PDF Sayfa 81: Range kullanımı
        public decimal Ucret { get; set; }

        public int SureDakika { get; set; } // Seans süresi
    }
}
