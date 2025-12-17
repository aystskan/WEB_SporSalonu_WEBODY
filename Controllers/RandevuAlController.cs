using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEBODY.Data;
using WEBODY.Models;

namespace WEBODY.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar (Üye veya Admin) erişebilir
    public class RandevuAlController : Controller
    {
        private readonly WebContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RandevuAlController(WebContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Randevu Formunu Göster
        public IActionResult Index()
        {
            // Dropdownlar için verileri hazırlıyoruz (ViewBag - PDF Sayfa 64)
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad");
            // Hizmetleri de seçtiriyoruz ki ücreti bilelim
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "HizmetId", "Ad");
            return View();
        }

        // POST: Randevuyu Kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("TarihSaat,AntrenorId")] Randevu randevu, int HizmetId)
        {
            // 1. Üye Bilgisini Al ve Ata
            var user = await _userManager.GetUserAsync(User);
            randevu.UyeAdSoyad = user.UserName;

            // 2. ModelState'den, formdan gelmeyen alanların hatalarını temizle
            // ÇÜNKÜ: Bu alanları biz kodla dolduruyoruz veya boş kalabilirler.
            ModelState.Remove("Antrenor");
            ModelState.Remove("UyeAdSoyad");
            ModelState.Remove("Durum");
            ModelState.Remove("HizmetAdi");

            // 3. Hizmet Kontrolü
            var secilenHizmet = await _context.Hizmetler.FindAsync(HizmetId);
            if (secilenHizmet == null)
            {
                ModelState.AddModelError("", "Lütfen bir hizmet seçiniz.");
                return YenidenYukle(randevu);
            }

            // SNAPSHOT MANTIĞI: O anki fiyatı ve adı randevuya kopyalıyoruz
            randevu.Ucret = secilenHizmet.Ucret;
            randevu.HizmetAdi = secilenHizmet.Ad;

            // 4. Tarih Kontrolü
            if (randevu.TarihSaat < DateTime.Now)
            {
                ModelState.AddModelError("TarihSaat", "Geçmiş bir tarihe randevu alamazsınız.");
                return YenidenYukle(randevu);
            }

            // Antrenörün çalışma saatlerini veritabanından çek
            var secilenAntrenor = await _context.Antrenorler.FindAsync(randevu.AntrenorId);

            if (secilenAntrenor != null)
            {
                int randevuSaati = randevu.TarihSaat.Hour; // Seçilen saati al (Örn: 14)

                // Eğer randevu saati, başlangıçtan küçükse VEYA bitişten büyük/eşitse hata ver
                if (randevuSaati < secilenAntrenor.BaslangicSaati || randevuSaati >= secilenAntrenor.BitisSaati)
                {
                    ModelState.AddModelError("TarihSaat",
                        $"Seçtiğiniz antrenör sadece {secilenAntrenor.BaslangicSaati}:00 ile {secilenAntrenor.BitisSaati}:00 saatleri arasında hizmet vermektedir.");
                    return YenidenYukle(randevu);
                }
            }

            // Bu üyenin bu saatte başka bir randevum var mı?
            bool uyeDoluMu = _context.Randevular.Any(r =>
                r.UyeAdSoyad == randevu.UyeAdSoyad &&
                r.TarihSaat == randevu.TarihSaat);

            if (uyeDoluMu)
            {
                ModelState.AddModelError("TarihSaat", "Bu tarih ve saatte zaten başka bir randevunuz var. Aynı anda iki yerde olamazsınız :)");
                return YenidenYukle(randevu);
            }

            // Aynı antrenörde, aynı saatte başka kayıt var mı?
            bool cakismaVarMi = _context.Randevular.Any(r =>
                r.AntrenorId == randevu.AntrenorId &&
                r.TarihSaat == randevu.TarihSaat);

            if (cakismaVarMi)
            {
                ModelState.AddModelError("TarihSaat", "Seçtiğiniz antrenör bu saatte dolu. Lütfen başka bir saat seçiniz.");
                return YenidenYukle(randevu);
            }

            // 6. KAYIT İŞLEMİ
            if (ModelState.IsValid)
            {
                randevu.Durum = "Beklemede";
                _context.Add(randevu);
                await _context.SaveChangesAsync();

                TempData["Mesaj"] = "Randevunuz alındı, onay bekleniyor.";

                // Başarılı olursa Randevularım sayfasına git
                return RedirectToAction(nameof(Randevularim));
            }

            // Hata varsa sayfayı tekrar göster (Hataları görmek için)
            return YenidenYukle(randevu);
        }

        // Helper: Hata durumunda formu tekrar dolduran metod
        private IActionResult YenidenYukle(Randevu randevu)
        {
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "HizmetId", "Ad");
            return View(randevu);
        }

        // Kullanıcının kendi randevularını göreceği sayfa
        public async Task<IActionResult> Randevularim()
        {
            var user = await _userManager.GetUserAsync(User);

            // --- OTOMATİK DURUM GÜNCELLEME ---
            // Tarihi geçmiş ama durumu hala "Onaylandı" veya "Beklemede" olanları bul
            var gecmisRandevular = await _context.Randevular
                .Where(r => r.TarihSaat < DateTime.Now
                       && r.Durum != "Tamamlandı"
                       && r.Durum != "İptal Edildi") // İptal edilenlere dokunma!
                .ToListAsync();

            if (gecmisRandevular.Any())
            {
                foreach (var randevu in gecmisRandevular)
                {
                    randevu.Durum = "Tamamlandı";
                }
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına işle
            }
            // ----------------------------------

            // Şimdi güncel listeyi çek
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Where(r => r.UyeAdSoyad == user.UserName)
                .OrderByDescending(r => r.TarihSaat)
                .ToListAsync();

            return View(randevular);
        }

        // 2. YENİ METOT: İPTAL ETME İŞLEMİ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IptalEt(int id)
        {
            // Randevuyu bul
            var randevu = await _context.Randevular.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            // Güvenlik Kontrolleri
            if (randevu == null)
            {
                return NotFound();
            }

            // Başkasının randevusunu iptal etmeye çalışıyorsa engelle
            if (randevu.UyeAdSoyad != user.UserName)
            {
                return Unauthorized(); // Yetkisiz Erişim
            }

            // Geçmiş randevu iptal edilemez
            if (randevu.TarihSaat < DateTime.Now)
            {
                TempData["Hata"] = "Geçmiş tarihli randevular iptal edilemez.";
                return RedirectToAction(nameof(Randevularim));
            }

            // Zaten iptal edilmişse işlem yapma
            if (randevu.Durum == "İptal Edildi")
            {
                TempData["Hata"] = "Bu randevu zaten iptal edilmiş.";
                return RedirectToAction(nameof(Randevularim));
            }

            // İptal İşlemi
            randevu.Durum = "İptal Edildi";
            _context.Update(randevu);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Randevunuz başarıyla iptal edildi.";
            return RedirectToAction(nameof(Randevularim));
        }
    }
}