using System.ComponentModel.DataAnnotations;

namespace WebProjeAyseT.Models
{
    public class Egitmen
    {
        [Key] // Birincil Anahtar [cite: 2450]
        public int EgitmenId { get; set; }

        [Display(Name = "Eğitmen Adı Soyadı")]
        
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")] // Zorunlu alan kontrolü [cite: 1999]
       
        [StringLength(100, ErrorMessage = "Maksimum 100 karakter girebilirsiniz.")] // Uzunluk kontrolü [cite: 2015]
        public string AdSoyad { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        [Required(ErrorMessage = "Uzmanlık alanı belirtilmelidir.")]
        public string UzmanlikAlani { get; set; } // Örn: Pilates, Fitness, Yoga

        // İlişki: Bir eğitmenin birden fazla randevusu olabilir (Navigation Property [cite: 2452])
        public virtual ICollection<Randevu> Randevular { get; set; }
    }
}
