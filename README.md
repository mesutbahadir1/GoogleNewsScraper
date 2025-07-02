# Google News Scraper
Google News Scraper, ücretli haber API’lerine alternatif olarak, verilen arama sorgusuna göre Google Haberler üzerinde otomatik arama yapıp, ilgili haber başlıklarını, bağlantılarını, görsellerini ve yayın tarihlerini ücretsiz şekilde çeken .NET Core tabanlı bir web servisidir.

# Özellikler
Google News sayfasını Selenium ile headless Chrome tarayıcıda gezerek haberleri kazır.

Haber başlığı, link, görsel ve yayın tarihi bilgilerini çıkarır.

Maksimum döndürülen haber sayısı varsayılan olarak 50 ile sınırlandırılmıştır, fakat bu değer ihtiyaç halinde değiştirilebilir.

Basit bir REST API üzerinden sorgu parametresi ile haber araması yapılabilir.

# Kullanım
API endpoint:
GET /api/news?query={arama_kelimesi}

Örnek:
GET /api/news?query=borsa

# Görsel 
<img width="1133" alt="image" src="https://github.com/user-attachments/assets/69d66198-e9cd-43a8-9f1f-53f01bc1b0c1" />

