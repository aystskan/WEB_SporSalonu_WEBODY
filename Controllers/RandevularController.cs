using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WEBODY.Data;
using WEBODY.Models;

namespace WEBODY.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    [Authorize(Roles = "Admin")]
    public class RandevularController : Controller
    {
        private readonly WebContext _context;

        public RandevularController(WebContext context)
        {
            _context = context;
        }

        // GET: Randevular
        public async Task<IActionResult> Index()
        {
            var webContext = _context.Randevular.Include(r => r.Antrenor);
            return View(await webContext.ToListAsync());
        }

        // GET: Randevular/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Antrenor)
                .FirstOrDefaultAsync(m => m.RandevuId == id);
            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // GET: Randevular/Create
        public IActionResult Create()
        {
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad");
            return View();
        }

        // POST: Randevular/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RandevuId,TarihSaat,AntrenorId,UyeAdSoyad,Durum")] Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(randevu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad", randevu.AntrenorId);
            return View(randevu);
        }

        // GET: Randevular/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu == null)
            {
                return NotFound();
            }
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad", randevu.AntrenorId);

            // Value: "Ad" (Çünkü veritabanına string isim kaydediyoruz)
            // Text: "Ad" (Görünecek kısım)
            ViewData["HizmetListesi"] = new SelectList(_context.Hizmetler, "Ad", "Ad", randevu.HizmetAdi);

            return View(randevu);
        }

        // POST: Randevular/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RandevuId,TarihSaat,AntrenorId,UyeAdSoyad,Durum,HizmetAdi,Ucret")] Randevu randevu)
        {
            if (id != randevu.RandevuId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // --- FİYAT GÜNCELLEME MANTIĞI ---
                    // Seçilen yeni hizmetin adını kullanarak Hizmetler tablosundan güncel fiyatını bulalım
                    var guncelHizmet = await _context.Hizmetler
                                             .FirstOrDefaultAsync(h => h.Ad == randevu.HizmetAdi);

                    if (guncelHizmet != null)
                    {
                        // Randevunun ücretini, yeni seçilen hizmetin ücretiyle değiştir
                        randevu.Ucret = guncelHizmet.Ucret;
                    }
                    // --------------------------------

                    _context.Update(randevu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RandevuExists(randevu.RandevuId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Hata olursa dropdownları tekrar doldur
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad", randevu.AntrenorId);
            ViewData["HizmetListesi"] = new SelectList(_context.Hizmetler, "Ad", "Ad", randevu.HizmetAdi); // Hata durumunda liste kaybolmasın

            return View(randevu);
        }

        // GET: Randevular/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var randevu = await _context.Randevular
                .Include(r => r.Antrenor)
                .FirstOrDefaultAsync(m => m.RandevuId == id);
            if (randevu == null)
            {
                return NotFound();
            }

            return View(randevu);
        }

        // POST: Randevular/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RandevuExists(int id)
        {
            return _context.Randevular.Any(e => e.RandevuId == id);
        }
    }
}
