using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WEBODY.Models;

namespace WEBODY.Data
{
    public class WebContext : IdentityDbContext
    {
        //Constructor
        public WebContext(DbContextOptions<WebContext> options) : base(options)
        {
        }

        public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
    }
}
