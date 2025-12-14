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

            // 3. Hizmet Kontrolü
            var secilenHizmet = await _context.Hizmetler.FindAsync(HizmetId);
            if (secilenHizmet == null)
            {
                ModelState.AddModelError("", "Lütfen bir hizmet seçiniz.");
                return YenidenYukle(randevu);
            }

            // 4. Tarih Kontrolü
            if (randevu.TarihSaat < DateTime.Now)
            {
                ModelState.AddModelError("TarihSaat", "Geçmiş bir tarihe randevu alamazsınız.");
                return YenidenYukle(randevu);
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
                randevu.Durum = "Onaylandı";
                _context.Add(randevu);
                await _context.SaveChangesAsync();

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

            // LINQ Where ile sadece oturum açan kişinin randevularını getir
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor) // Join işlemi (İlişkili veriyi getir)
                .Where(r => r.UyeAdSoyad == user.UserName)
                .OrderByDescending(r => r.TarihSaat) // En yeni en üstte
                .ToListAsync();

            return View(randevular);
        }
    }
}