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
        // Google AI Studio'dan aldığın API Anahtarını buraya yapıştır
        private const string ApiKey = "AIzaSyA192umV7Eh-R-KHR4d_3Rqed0rT5LnDYA";

        // Google Gemini API Adresi
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent";

        [HttpGet]
        public IActionResult Index()
        {
            return View(new YapayZekaViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(YapayZekaViewModel model)
        {
            // Kullanıcı boş veri gönderirse API'yi yormayalım
            if (string.IsNullOrEmpty(model.Hedef) || model.Kilo == 0)
            {
                ViewBag.Hata = "Lütfen tüm alanları doldurunuz.";
                return View(model);
            }

            // 1. Prompt (İstem) Hazırlığı: Yapay Zekaya ne soracağız?
            string prompt = $@"
                Sen uzman bir sporcu, antrenör ve diyetisyensin.
                Danışan Bilgileri:
                - Yaş: {model.Yas}, Boy: {model.Boy} cm, Kilo: {model.Kilo} kg, Cinsiyet: {model.Cinsiyet}
                - Hedef: {model.Hedef}
                - Aktivite: {model.AktiviteDuzeyi}

                Görev:
                Bu kişi için HTML formatında (Bootstrap classları kullanmadan, sadece <h4>, <ul>, <li>, <p> etiketleri ile)
                1. Kısa bir Durum Analizi (Vücut kitle indeksi yorumu vb.)
                2. 3 maddelik temel beslenme tavsiyesi
                3. Etkili ve detaylı egzersiz tavsiyesi
                4. Hangi egzesizleri yapınca nasıl görünecekleri yönünde bir resim
                hazırla.
                Cevabı sadece HTML içeriği olarak ver, markdown (```html) kullanma.
                Samimi ve motive edici bir dil kullan.";

            // 2. HTTP İsteği Hazırlığı
            using (var client = new HttpClient())
            {
                // Gemini API'nin beklediği JSON formatı
                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                var jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                try
                {
                    // URL'in sonuna API Key'i ekleyerek POST isteği atıyoruz
                    var response = await client.PostAsync($"{ApiUrl}?key={ApiKey}", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        // Gelen JSON verisini parçala ve cevabı al
                        dynamic result = JsonConvert.DeserializeObject(responseString);

                        // Gemini'nin cevabı şu yolda bulunur:
                        string aiResponse = result?.candidates?[0]?.content?.parts?[0]?.text;

                        if (!string.IsNullOrEmpty(aiResponse))
                        {
                            // Markdown temizliği (Bazen ```html ile sarılı gelir, temizleyelim)
                            aiResponse = aiResponse.Replace("```html", "").Replace("```", "");
                            model.YapayZekaCevabi = aiResponse;
                        }
                        else
                        {
                            ViewBag.Hata = "Yapay zeka boş bir cevap döndürdü.";
                        }
                    }
                    else
                    {
                        // Hata durumunda (Örn: API Key yanlışsa)
                        ViewBag.Hata = $"Servis Hatası: {response.StatusCode} - Lütfen API Key'i kontrol ediniz.";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Hata = $"Bağlantı Hatası: {ex.Message}";
                }
            }

            return View(model);
        }
    }
}
