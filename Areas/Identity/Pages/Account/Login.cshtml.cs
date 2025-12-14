using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WEBODY.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                string emailClean = Input.Email.Trim().ToLower();
                string passwordClean = Input.Password.Trim();

                // 1. Kullanıcıyı Bul
                var user = await _userManager.FindByEmailAsync(emailClean);

                if (user != null)
                {
                    // --- DİNAMİK ROL SİSTEMİ ---
                    // Önce kullanıcının mevcut tüm rollerini temizle (Sıfırla)
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    // Şifre "sau" mu ve Sakarya uzantılı mı?
                    if ((emailClean.EndsWith("@sakarya.edu.tr") || emailClean.EndsWith("@ogr.sakarya.edu.tr"))
                        && passwordClean == "sau")
                    {
                        // Evet: O zaman bu oturum için ADMIN olsun
                        if (!await _roleManager.RoleExistsAsync("Admin")) await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        await _userManager.AddToRoleAsync(user, "Admin");

                        // Şifre kontrolü yapmadan direkt içeri al (Çünkü "sau" master key gibi davranıyor)
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User logged in as ADMIN with master password.");
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        // Hayır: O zaman normal ÜYE olsun
                        if (!await _roleManager.RoleExistsAsync("Uye")) await _roleManager.CreateAsync(new IdentityRole("Uye"));
                        await _userManager.AddToRoleAsync(user, "Uye");

                        // Normal şifre kontrolüne devam et
                    }
                }
                // Kullanıcı yoksa (İlk defa "sau" ile geliyorsa) oluştur ve Admin yap
                else if ((emailClean.EndsWith("@sakarya.edu.tr") || emailClean.EndsWith("@ogr.sakarya.edu.tr"))
                         && passwordClean == "sau")
                {
                    var newAdmin = new IdentityUser
                    {
                        UserName = emailClean,
                        Email = emailClean,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(newAdmin, "sau");
                    if (createResult.Succeeded)
                    {
                        if (!await _roleManager.RoleExistsAsync("Admin")) await _roleManager.CreateAsync(new IdentityRole("Admin"));
                        await _userManager.AddToRoleAsync(newAdmin, "Admin");

                        await _signInManager.SignInAsync(newAdmin, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                // --- STANDART GİRİŞ KONTROLÜ ---
                // (Buraya sadece normal şifreyle girmeye çalışanlar veya Üye rolü atananlar düşer)
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in as MEMBER.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Giriş başarısız. Kullanıcı adı veya şifre hatalı.");
                    return Page();
                }
            }

            return Page();
        }
    }
}