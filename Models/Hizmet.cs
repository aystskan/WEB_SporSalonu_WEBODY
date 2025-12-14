using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Bu kütüphaneyi eklemelisin!

namespace WEBODY.Models
{
    public class Hizmet
    {
        [Key]
        public int HizmetId { get; set; }

        [Required(ErrorMessage = "Hizmet adı boş geçilemez.")]
        [Display(Name = "Hizmet Adı")]
        public string Ad { get; set; }

        // --- GÜNCELLENEN KISIM ---
        [Range(0, 10000, ErrorMessage = "Ücret 0 ile 10.000 TL arasında olmalıdır.")]
        [Column(TypeName = "decimal(18, 2)")] // SQL'de Money tipi gibi davranmasını sağlar
        public decimal Ucret { get; set; }
        // -------------------------

        public int SureDakika { get; set; }
    }
}