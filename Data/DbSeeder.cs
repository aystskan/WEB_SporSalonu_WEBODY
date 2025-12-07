using Microsoft.AspNetCore.Identity;
using WebProjeAyseT.Models;

namespace WebProjeAyseT.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. ROLLERİ OLUŞTUR (Yoksa Ekle)
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Uye"))
                await roleManager.CreateAsync(new IdentityRole("Uye"));

            // 2. ANA ADMİNİ OLUŞTUR (Sizin Hesabınız)
            // Bu hesap veritabanı silinse bile proje her başladığında tekrar oluşturulur.
            var myAdminEmail = "g231210001@sakarya.edu.tr"; // KENDİ MAİLİNİZİ YAZIN

            var adminUser = await userManager.FindByEmailAsync(myAdminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = myAdminEmail,
                    Email = myAdminEmail,
                    EmailConfirmed = true
                };

                // Şifre: sau
                var result = await userManager.CreateAsync(newAdmin, "sau");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
