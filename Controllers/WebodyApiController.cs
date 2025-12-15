using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ToListAsync için gerekli
using WEBODY.Data;
using WEBODY.Models;

namespace WEBODY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebodyApiController : ControllerBase
    {
        private readonly WebContext _context;

        public WebodyApiController(WebContext context)
        {
            _context = context;
        }

        // 1. ENDPOINT: Tüm Antrenörleri Getir (LINQ - Select Kullanımı)
        // İstek Adresi: https://localhost:port/api/WebodyApi/Antrenorler
        [HttpGet("Antrenorler")]
        public async Task<IActionResult> GetAntrenorler()
        {
            // LINQ Select ile sadece ihtiyacımız olan verileri çekiyoruz (Data Shaping)
            var antrenorler = await _context.Antrenorler
                .Select(a => new
                {
                    Id = a.AntrenorId,
                    Isim = a.AdSoyad,
                    // İlişkili verileri (Randevular vb.) çekmeyerek döngüye girmeyi engelliyoruz
                })
                .ToListAsync();

            return Ok(antrenorler);
        }
        [HttpGet("GecmisRandevular")]
        public async Task<IActionResult> GetGecmisRandevular()
        {
            // LINQ Sorgusu:
            // 1. Where: Tarihi şu andan küçük olanları filtrele.
            // 2. OrderBy: Tarihe göre sırala.
            // 3. Select: Anonim nesne oluştur (Frontend'in anlayacağı basit format).

            var randevular = await _context.Randevular
                .Include(r => r.Antrenor) // Join işlemi
                .Where(r => r.TarihSaat < DateTime.Now)
                .OrderBy(r => r.TarihSaat)
                .Select(r => new
                {
                    Tarih = r.TarihSaat.ToString("dd.MM.yyyy HH:mm"),
                    AntrenorAdi = r.Antrenor != null ? r.Antrenor.AdSoyad : "Silinmiş Antrenör",
                    Uye = r.UyeAdSoyad,
                    Durum = r.Durum
                })
                .ToListAsync();

            if (!randevular.Any())
            {
                return NotFound("Gelecek tarihe ait randevu bulunamadı.");
            }

            return Ok(randevular);
        }
        // 3. ENDPOINT: Gelecek Randevuları Getir (LINQ - Where ve OrderBy Kullanımı)
        // İstek Adresi: https://localhost:port/api/WebodyApi/GelecekRandevular
        [HttpGet("GelecekRandevular")]
        public async Task<IActionResult> GetGelecekRandevular()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor) // Join işlemi
                .Where(r => r.TarihSaat >= DateTime.Now)
                .OrderBy(r => r.TarihSaat)
                .Select(r => new
                {
                    Tarih = r.TarihSaat.ToString("dd.MM.yyyy HH:mm"),
                    AntrenorAdi = r.Antrenor != null ? r.Antrenor.AdSoyad : "Silinmiş Antrenör",
                    Uye = r.UyeAdSoyad,
                    Durum = r.Durum
                })
                .ToListAsync();

            if (!randevular.Any())
            {
                return NotFound("Gelecek tarihe ait randevu bulunamadı.");
            }

            return Ok(randevular);
        }
    }
}