using Microsoft.EntityFrameworkCore;
using WEBODY.Models;

namespace WEBODY.Data
{
    public class WebContext : DbContext
    {
        public WebContext(DbContextOptions<WebContext> options) : base(options)
        {
        }

        [cite_start]//Tabloların temsili (DbSet) 
        public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
    }
}
