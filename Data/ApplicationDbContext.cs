using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebProjeAyseT.Models; // Modellerin olduğu namespace

namespace WebProjeAyseT.Data
{
    public class ApplicationDbContext : IdentityDbContext // Identity entegrasyonu (Rol bazlı giriş) kullanacağımız için DbContext yerine IdentityDbContext sınıfından miras alacağız.
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tabloların veritabanındaki karşılıkları 
        public DbSet<Egitmen> Egitmenler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity tabloları için bu satır ZORUNLUDUR!

            // Ucret alanı için hassasiyet ayarı (Toplam 18 basamak, 2'si ondalık)
            builder.Entity<Hizmet>()
                .Property(h => h.Ucret)
                .HasColumnType("decimal(18,2)");
        }
    }
}
