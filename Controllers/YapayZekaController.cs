using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Newtonsoft.Json;
using WEBODY.Models;

namespace WEBODY.Controllers
{
    [Authorize] // Sadece üyeler erişebilsin
    public class YapayZekaController : Controller
    {
        // Google AI Studio
        private const string ApiKey = "AIzaSyDc1Qq_eFfBcwJWJ3eJp-6f1xk3ytXkor4";

        // Google Gemini API Adresi
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new YapayZekaViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(YapayZekaViewModel model)
        {
            if (model.Kilo == 0 || model.Boy == 0)
            {
                ViewBag.Hata = "Lütfen boy ve kilo bilgilerini giriniz.";
                return View(model);
            }

            // --- 1. KULLANICI DETAYLARI (İngilizce Çeviri) ---
            string cinsiyetIng = model.Cinsiyet == "Kadın" ? "woman" : "man";

            // Sakal durumu (Sadece erkekse ve seçildiyse)
            string sakal = (model.Cinsiyet == "Erkek" && model.SakalVarMi == "Beard") ? "with beard" : "clean shaven";

            // Gözlük durumu
            string gozluk = (model.GozlukVarMi == "Wearing Glasses") ? "wearing glasses" : "no glasses";
            string kiyafet = "wearing full body modest for outdoor sports, sport tshirt and long outdoor sport pant, professional gym attire";

            // --- 2. HİBRİT PROMPT ---
            string prompt = $@"
                Sen uzman bir sporcu, antrenör ve diyetisyensin.                
                Danışan Profili:
                - Fiziksel: {model.Yas} yaş, {model.Boy}cm, {model.Kilo}kg, {model.Cinsiyet}
                - Görünüm Detayları: {model.SacRengi} saçlı, {model.TenRengi} tenli, {gozluk}, {sakal}.
                - Hedef: {model.Hedef}
                - Aktivite Seviyesi: {model.AktiviteDuzeyi}

                GÖREVİN:
                Bu kişi için HTML formatında, görsel destekli profesyonel bir program hazırla.

                ADIM 1: PROGRAMI BELİRLE
                Bu kişi için en uygun 3 temel egzersizi kafanda belirle.

                Egzersiz açıklamaları detaylı, kolay anlaşılır ve destekleyici olsun.
                Tüm egzersiz isimleri ve kas grupları MUTLAKA şu formatta yazılmalı: ""Latince(/İngilizce) Adı/ Türkçe Adı)"".
                Örnek: ""Pectoralis Major/ Büyük Göğüs Kası"", ""Bench Press/ Göğüs İtiş""

                --- DOLDURMAN GEREKEN HTML ŞABLONU ---

                <div class='mb-4'>
                    <h4 class='fw-bold text-primary'><i class='bi bi-bar-chart-fill'></i> Durum Analizi</h4>
                    <div class='alert alert-light border-start border-4 border-primary shadow-sm'>
                        <p class='mb-0'>[BURAYA KİŞİNİN ANALİZİNİ VE MOTİVASYON YAZISINI YAZ]</p>
                    </div>
                </div>

                <div class='mb-4'>
                    <h4 class='fw-bold text-success'><i class='bi bi-basket-fill'></i> Beslenme Tavsiyeleri</h4>
                    <ul class='list-group list-group-flush shadow-sm rounded'>
                        <li class='list-group-item'><i class='bi bi-check-circle-fill text-success me-2'></i> [GENEL TAVSİYE 1 (Örn: Protein alımı hk.)]</li>
                        <li class='list-group-item'><i class='bi bi-check-circle-fill text-success me-2'></i> [GENEL TAVSİYE 2 (Örn: Sıvı tüketimi hk.)]</li>
                        <li class='list-group-item'><i class='bi bi-check-circle-fill text-success me-2'></i> [GENEL TAVSİYE 3 (Örn: Şeker/Karbonhidrat ve beslenme saatleri hk.)]</li>
                    </ul>
                </div>

                <div class='mb-4'>
                    <h4 class='fw-bold text-danger'><i class='bi bi-activity'></i> Antrenman Programı</h4>
                    <div class='list-group shadow-sm'>
                        <div class='list-group-item list-group-item-action'>
                            <div class='d-flex w-100 justify-content-between'>
                                <h5 class='mb-1 fw-bold'>[1. EGZERSİZ ADI (TÜRKÇE / İNGİLİZCE)]</h5>
                            </div>
                            <p class='mb-1'>[NASIL YAPILIR KISA AÇIKLAMA]</p>
                            <small class='text-muted'><em>Hedef Bölge:</em> [ÇALIŞAN KASLAR (TÜRKÇE / LATİNCE)]</small>
                        </div>
                        
                        <div class='list-group-item list-group-item-action'>
                            <div class='d-flex w-100 justify-content-between'>
                                <h5 class='mb-1 fw-bold'>[2. EGZERSİZ ADI (TÜRKÇE / İNGİLİZCE)]</h5>
                            </div>
                            <p class='mb-1'>[NASIL YAPILIR KISA AÇIKLAMA]</p>
                            <small class='text-muted'><em>Hedef Bölge:</em> [ÇALIŞAN KASLAR]</small>
                        </div>

                        <div class='list-group-item list-group-item-action'>
                            <div class='d-flex w-100 justify-content-between'>
                                <h5 class='mb-1 fw-bold'>[3. EGZERSİZ ADI (TÜRKÇE / İNGİLİZCE)]</h5>
                            </div>
                            <p class='mb-1'>[NASIL YAPILIR KISA AÇIKLAMA]</p>
                            <small class='text-muted'><em>Hedef Bölge:</em> [ÇALIŞAN KASLAR]</small>
                        </div>
                    </div>
                </div>

                ADIM 2: HEDEF VÜCUT GÖRSELİ OLUŞTUR (Pollinations AI Linki)
                Belirlediğin bu 3 egzersizin çalıştırdığı ana kas gruplarını düşün.
                Ardından, kullanıcının yukarıdaki 'Görünüm Detayları'na birebir uyan ({gozluk} detayı dahil), 
                ancak senin belirlediğin o kas grupları hedefe uygun şekilde gelişmiş,
                spor kıyafetleri (altında outdoor spor pantalonu) içinde, kendine güvenen, AYAKTAN BAŞA TAM VÜCUT (FULL BODY SHOT),
                DOĞAL VE GERÇEKÇİ (Photorealistic) bir fotoğrafını oluşturacak Pollinations URL'i hazırla.

                URL Formatı: https://image.pollinations.ai/prompt/[İNGİLİZCE_TARİF]?width=400&height=600&model=flux
                (Tarifin içine 'hyperrealistic', '4k', 'natural lighting', 'outdoor sports pants', 'full body shot', '{gozluk}','{kiyafet}' kelimelerini mutlaka ekle).

                ÇIKTI FORMATI:
                Önce yukarıdaki doldurulmuş HTML kodunu ver.
                HTML bittikten sonra en sona '|||' işaretini koy ve hemen peşine oluşturduğun URL'i yapıştır.

                Samimi ve motive edici, disiplin sahibi bir dil kullan.
            ";

            // --- API İSTEĞİ ---
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync($"{ApiUrl}?key={ApiKey}", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject(responseString);
                        string aiResponse = result?.candidates?[0]?.content?.parts?[0]?.text;

                        if (!string.IsNullOrEmpty(aiResponse))
                        {
                            // Markdown temizliği
                            string cleanResponse = aiResponse.Replace("```html", "").Replace("```", "").Trim();

                            // BÖLME İŞLEMİ (HTML ve Link Ayrımı)
                            if (cleanResponse.Contains("|||"))
                            {
                                var parts = cleanResponse.Split(new[] { "|||" }, StringSplitOptions.None);
                                model.YapayZekaCevabi = parts[0].Trim(); // Metin
                                model.AvatarUrl = parts[1].Trim();      // Link
                            }
                            else
                            {
                                model.YapayZekaCevabi = cleanResponse;
                            }
                        }
                        else
                        {
                            ViewBag.Hata = "Yapay zeka yanıt üretemedi. Lütfen tekrar deneyin.";
                        }
                    }
                    else
                    {
                        ViewBag.Hata = $"API Hatası: {response.StatusCode}. Lütfen biraz bekleyip tekrar deneyin.";
                    }
                }
                catch (TaskCanceledException)
                {
                    ViewBag.Hata = "Yapay zekanın görsel oluşturması uzun sürdü, zaman aşımına uğradı. Tekrar deneyin.";
                }
                catch (Exception ex)
                {
                    ViewBag.Hata = "Bir hata oluştu: " + ex.Message;
                }
            }

            return View(model);
        }
    }
}