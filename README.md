Projeden İstenenler
--------------------
Projenin amacı, Web Programlama dersinde teorik ve pratik olarak öğrenilen bilgilerin, gerçek bir probleme
uygulanarak bu probleme çözüm üreten bir web projesi geliştirilmesidir.

Proje Hakkında Kısa Açıklama
-----------------------------
Bu projede, ASP.NET Core MVC kullanarak bir Spor Salonu (Fitness Center) Yönetim ve Randevu Sistemi
geliştirilmesi hedeflenmektedir. Sistem, spor salonlarının sunduğu hizmetleri, antrenörlerin uzmanlık alanlarını,
üyelerin randevularını ve yapay zekâ tabanlı egzersiz önerilerini yönetebilecek nitelikte olacaktır. Bu sayede
hem antrenörlerin verimliliği hem de üyelerin kişiselleştirilmiş spor deneyimleri izlenebilecektir.

Proje Konsept ve Gereksinimler
-------------------------------
1. Antrenör (Eğitmen) Yönetimi:
   
• Salonda görev yapan antrenörler sisteme tanımlanabilmekte.
• Her bir antrenörün uzmanlık alanları (örneğin kas geliştirme, kilo verme vb.) ve yapabildiği
hizmet türleri belirtilmiştir.
• Antrenörlerin müsaitlik saatleri tanımlanarak, üyeler bu zaman aralıklarına göre randevu
alabilmektedir.


2. Üye ve Randevu Sistemi:

• Kullanıcılar (üyeler), uygun antrenör ve hizmete göre sistem üzerinden randevu alabilmektedir.
• Randevu saati, önceki randevular dikkate alınarak uygun değilse sistem kullanıcıyı uyarır.
• Randevu detayları (hizmet, süre, ücret, eğitmen bilgisi) sistemde saklanır.
• Randevu onay mekanizması bulunmaktadır.


3. Raporlama - REST API Kullanımı:

• REST API kullanılarak veritabanı ile iletişim sağlanmıştır.
• API üzerinden LINQ sorguları ile filtreleme birden fazla modülde gerçekleştirilmiştir.


4. Yapay Zeka Entegrasyonu:
   
• Projede bir yapay zekâ aracı ile entegre çalışan bir özellik bulunmaktadır.
• Kullanıcılar, sisteme vücut yaş/boy/kilo bilgisi girerek, yapay zekâ
aracılığıyla kendilerine uygun egzersiz veya diyet planı önerileri alabilmeleri, hangi egzesizleri
yapınca nasıl görünecekleri yönünde bir resim istenmesi vb.
