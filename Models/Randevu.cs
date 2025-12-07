using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ForeignKey için gerekli

namespace WebProjeAyseT.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuId { get; set; }

        [Display(Name = "Randevu Tarihi")]
        [Required]
        public DateTime TarihSaat { get; set; }

        [Display(Name = "Durum")]
        public string Durum { get; set; } = "Beklemede"; // Onaylandı, İptal, Beklemede

        // İlişkiler (Foreign Keys) - Hangi eğitmen? [cite: 2452]
        [ForeignKey("Egitmen")]
        public int EgitmenId { get; set; }
        public virtual Egitmen Egitmen { get; set; }

        // İlişkiler - Hangi hizmet?
        [ForeignKey("Hizmet")]
        public int HizmetId { get; set; }
        public virtual Hizmet Hizmet { get; set; }

        // İlişkiler - Hangi üye? (Identity User Id string olarak tutulur)
        public string UyeId { get; set; }
    }
}
