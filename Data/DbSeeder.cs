using Microsoft.AspNetCore.Identity;
using WEBODY.Models;

namespace WEBODY.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. ROLLERİN KONTROLÜ VE OLUŞTURULMASI (BU KISIM ŞART)
            // Eğer "Admin" rolü yoksa oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            // Eğer "Uye" rolü yoksa oluştur
            if (!await roleManager.RoleExistsAsync("Uye"))
                await roleManager.CreateAsync(new IdentityRole("Uye"));

            // 2. ANA ADMİN (SEN) İÇİN GARANTİ OLUŞTURMA (OPSİYONEL AMA İYİDİR)
            // Login sayfasındaki kod zaten yapacak ama veritabanı oluşur oluşmaz senin hesabın hazır olsun.
            string myAdminEmail = "g231210043@sakarya.edu.tr";

            var userCheck = await userManager.FindByEmailAsync(myAdminEmail);
            if (userCheck == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = myAdminEmail,
                    Email = myAdminEmail,
                    EmailConfirmed = true,
                };

                // Varsayılan şifre: sau
                var result = await userManager.CreateAsync(newAdmin, "sau");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}