using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WEBODY.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<WebContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // ŞİFRE KURALLARI (sau için)
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
.AddRoles<IdentityRole>() // Rolleri aktif ediyoruz (Admin/Uye icin)
.AddEntityFrameworkStores<WebContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();// Kimlik Do�rulama (Kimsin?)
app.UseAuthorization();// Yetkilendirme (Neye yetkin var?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Identity sayfalar�n�n (Login/Register) �al��mas� i�in �art!
app.MapRazorPages(); 

// VERİTABANI TOHUMLAMA (SEEDING) İŞLEMİ
// Uygulama her başladığında çalışır, Admin yoksa oluşturur.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbSeeder.SeedRolesAndAdminAsync(services);
}

app.Run();
