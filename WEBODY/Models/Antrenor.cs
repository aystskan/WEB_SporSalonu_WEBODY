using System.ComponentModel.DataAnnotations; //Doğrulama için gerekli namespace

namespace WEBODY.Models
{
    public class Antrenor
    {
        [Key] //Primary Key
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")] //Zorunlu alan
        [StringLength(100, ErrorMessage = "En fazla 100 karakter girebilirsiniz.")] // PDF Sayfa 82
        [Display(Name = "Antrenör Adı Soyadı")]
        public string AdSoyad { get; set; }

        [Required]
        [Display(Name = "Uzmanlık Alanı")]
        public string UzmanlikAlani { get; set; } // Örn: Yoga, Pilates, BodyBuilding

        // Bir antrenörün birden fazla randevusu olabilir (Navigation Property - PDF Sayfa 108)
        public ICollection<Randevu> Randevular { get; set; }
    }
}


