using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ForeignKey için gerekli

namespace WEBODY.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuId { get; set; }

        [Required]
        [Display(Name = "Randevu Tarihi")]
        public DateTime TarihSaat { get; set; }

        [cite_start]// İlişkiler (PDF Sayfa 108: Navigation Properties) [cite: 2452]
        
        [Display(Name = "Antrenör")]
        public int AntrenorId { get; set; }
        [ForeignKey("AntrenorId")]
        public Antrenor Antrenor { get; set; }

        // Üye bilgisi Identity kütüphanesinden gelecek ancak şimdilik basit tutuyoruz.
        [Display(Name = "Üye Adı")]
        [Required]
        public string UyeAdSoyad { get; set; } 
        
        public string Durum { get; set; } = "Beklemede"; // Onaylandı, İptal vs.
    }
}
