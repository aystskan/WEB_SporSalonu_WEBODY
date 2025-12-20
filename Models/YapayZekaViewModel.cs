namespace WEBODY.Models
{
    public class YapayZekaViewModel
    {
        // Kullanıcıdan alınacak veriler
        public int Yas { get; set; }
        public int Boy { get; set; } // cm cinsinden
        public int Kilo { get; set; }
        public string Cinsiyet { get; set; } // Kadın/Erkek
        public string Hedef { get; set; } // Kilo Verme, Kas Yapma, Form Koruma
        public string AktiviteDuzeyi { get; set; } // Hareketsiz, Orta, Çok Hareketli

        // --- FİZİKSEL ÖZELLİKLER (AVATAR İÇİN) ---
        public string SacRengi { get; set; } // Örn: Siyah, Sarı, Kel
        public string TenRengi { get; set; } // Örn: Açık, Buğday, Esmer
        public string GozlukVarMi { get; set; } // Evet/Hayır
        public string SakalVarMi { get; set; } // Evet/Hayır (Erkekse)

        // Yapay Zekadan dönecek cevap
        public string? YapayZekaCevabi { get; set; }

        public string? AvatarUrl { get; set; }
    }
}
