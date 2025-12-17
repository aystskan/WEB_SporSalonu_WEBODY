using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ForeignKey için gerekli

namespace WEBODY.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuId { get; set; }

        [Required(ErrorMessage = "Tarih ve Saat seçimi zorunludur.")]
        [Display(Name = "Randevu Tarihi")]
        public DateTime TarihSaat { get; set; }

        [Display(Name = "Alınan Hizmet")]
        public string? HizmetAdi { get; set; }

        [Display(Name = "Antrenör")]
        public int AntrenorId { get; set; }
        [ForeignKey("AntrenorId")]
        public Antrenor? Antrenor { get; set; }

        // Üye bilgisi Identity kütüphanesinden gelecek ancak şimdilik basit tutuyoruz.
        [Display(Name = "Üye Adı")]
        [Required]
        public string? UyeAdSoyad { get; set; }

        public string Durum { get; set; } = "Beklemede"; // Onaylandı, İptal vs.

        [Display(Name = "Ücret")]
        [Column(TypeName = "decimal(18,2)")] // Para birimi formatı
        public decimal Ucret { get; set; }
    }
}
