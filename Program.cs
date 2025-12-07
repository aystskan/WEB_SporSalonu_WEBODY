using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProjeAyseT.Data;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================
// 1. VERÝTABANI VE KÝMLÝK (IDENTITY) SERVÝSLERÝNÝN EKLENMESÝ
// ==================================================================

// Baðlantý cümlesini (Connection String) okuma
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Baðlantý dizesi 'DefaultConnection' bulunamadý.");

// DbContext'i SQL Server ile yapýlandýrma
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity (Üyelik ve Rol) Sistemini ekleme ve ayarlarý gevþetme
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Geliþtirme aþamasýnda kolaylýk olmasý için þifre kurallarý:
    options.Password.RequireDigit = false;           // Rakam zorunlu deðil
    options.Password.RequiredLength = 3;             // En az 3 karakter (sau için)
    options.Password.RequireLowercase = false;       // Küçük harf zorunlu deðil
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu deðil
    options.Password.RequireNonAlphanumeric = false; // Sembol (!,*,vb.) zorunlu deðil

    // Oturum açma ayarlarý (isteðe baðlý)
    options.SignIn.RequireConfirmedAccount = false; // E-posta onayý zorunluluðunu kapattýk
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// MVC (Controller ve View) servislerini ekleme
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==================================================================
// 2. HTTP ÝSTEK HATTI (PIPELINE) YAPILANDIRMASI
// ==================================================================

// Hata yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // HTTP isteklerini HTTPS'e yönlendir
app.UseStaticFiles();      // wwwroot klasörünü (css, js, img) dýþa aç

app.UseRouting();          // Yönlendirme mekanizmasýný baþlat

// --- KÝMLÝK DOÐRULAMA VE YETKÝLENDÝRME (SIRASI ÖNEMLÝDÝR) ---
app.UseAuthentication(); // 1. Kimsin? (Giriþ yapmýþ mý?)
app.UseAuthorization();  // 2. Yetkin var mý? (Admin mi?)

// Varsayýlan Rota Tanýmlamasý (Home/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==================================================================
// 3. VERÝTABANI TOHUMLAMA (SEED DATA) ÝÞLEMÝ
// ==================================================================
// Uygulama her baþladýðýnda çalýþýr: Roller ve Admin yoksa oluþturur.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // DbSeeder sýnýfýndaki metodu çaðýrýyoruz
        await DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // Olasý bir hata durumunda loglama yapýlabilir
        Console.WriteLine("Seed iþlemi sýrasýnda hata oluþtu: " + ex.Message);
    }
}

app.Run();