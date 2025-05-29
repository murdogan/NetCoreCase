# İçerik Yönetim Sistemi (CMS)

## Proje Genel Bakış

Bu proje, modern .NET Core teknolojileri kullanılarak geliştirilmiş kapsamlı bir İçerik Yönetim Sistemi (CMS)'dir. Sistem, kullanıcıların içeriklerle etkileşim kurmasına, içerik varyantlarını yönetmesine ve kategorilere göre filtreleyerek görüntülemesine olanak tanır.

## Özellikler

### Temel Özellikler
- ✅ **Kullanıcı Yönetimi**: Kullanıcı profilleri ve kimlik doğrulama
- ✅ **İçerik Yönetimi**: Kategorilere göre organize edilmiş içerikler
- ✅ **Varyant Sistemi**: Her içerik için birden fazla varyant desteği
- ✅ **Çoklu Dil Desteği**: İçeriklerin farklı dillerde sunumu
- ✅ **Kategori Filtreleme**: İçeriklerin kategorilere göre filtrelenmesi
- ✅ **Performans Optimizasyonu**: Redis/In-Memory cache desteği

### Teknik Özellikler
- ✅ **SOLID Prensipleri**: Temiz ve sürdürülebilir kod yapısı
- ✅ **Katmanlı Mimari**: Domain, Application, Infrastructure katmanları
- ✅ **Repository Pattern**: Veri erişim katmanının soyutlanması
- ✅ **Unit of Work**: Transaksiyonel işlemler için
- ✅ **DTO Pattern**: Veri transfer nesneleri
- ✅ **Mapster**: Nesne eşleme
- ✅ **Async/Await**: Asenkron programlama
- ✅ **Cache Yönetimi**: 15 dakikalık cache süresi

## Teknoloji Stack

### Backend
- **Framework**: .NET Core 8.0
- **Veritabanı**: Entity Framework Core (SQL Server/PostgreSQL)
- **Cache**: Redis / In-Memory Cache
- **Mapping**: Mapster
- **IoC Container**: Built-in DI Container

### Geliştirme Araçları
- **IDE**: Visual Studio 2022 / Visual Studio Code
- **Package Manager**: NuGet
- **Version Control**: Git

## Proje Mimarisi

```
NetCoreCase/
├── src/
│   ├── NetCoreCase.Domain/           # Domain katmanı (Entities, Value Objects)
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Content.cs
│   │   │   ├── Category.cs
│   │   │   └── ContentVariant.cs
│   │   └── Interfaces/
│   │
│   ├── NetCoreCase.Application/      # Application katmanı (Services, DTOs)
│   │   ├── DTOs/
│   │   ├── Services/
│   │   │   ├── UserService.cs
│   │   │   └── ContentService.cs
│   │   └── Interfaces/
│   │
│   ├── NetCoreCase.Infrastructure/   # Infrastructure katmanı (Data Access, External)
│   │   ├── Data/
│   │   │   ├── Repositories/
│   │   │   └── ApplicationDbContext.cs
│   │   ├── Cache/
│   │   └── Mapping/
│   │
│   └── NetCoreCase.API/             # Presentation katmanı (Controllers, API)
│       ├── Controllers/
│       ├── Middleware/
│       └── Program.cs
│
├── tests/
│   ├── NetCoreCase.UnitTests/
│   └── NetCoreCase.IntegrationTests/
│
└── docs/                            # Dokümantasyon
```

## Veri Modeli

### Entities

#### User (Kullanıcı)
```csharp
- Id: Guid
- FullName: string
- Email: string
- CreatedAt: DateTime
- Contents: List<Content>
```

#### Content (İçerik)
```csharp
- Id: Guid
- Title: string
- Description: string
- CategoryId: Guid
- Language: string (en, tr)
- ImageUrl: string
- UserId: Guid
- CreatedAt: DateTime
- Variants: List<ContentVariant>
```

#### Category (Kategori)
```csharp
- Id: Guid
- Name: string
- Description: string
- Contents: List<Content>
```

#### ContentVariant (İçerik Varyantı)
```csharp
- Id: Guid
- ContentId: Guid
- VariantData: string
- IsDefault: bool
```

## Kurulum

### Gereksinimler
- .NET Core 8.0 SDK
- SQL Server / PostgreSQL
- Redis (isteğe bağlı, In-Memory cache kullanılabilir)
- Visual Studio 2022 veya VS Code

### Adım 1: Repository'yi Klonlayın
```bash
git clone <repository-url>
cd NetCoreCase
```

### Adım 2: Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### Adım 3: Veritabanı Yapılandırması
`appsettings.json` dosyasında connection string'i düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NetCoreCaseDB;Trusted_Connection=true;"
  }
}
```

### Adım 4: Veritabanı Migration'ları
```bash
dotnet ef database update
```

### Adım 5: Projeyi Çalıştırın
```bash
dotnet run --project src/NetCoreCase.API
```

## API Endpoints

### Kullanıcı Yönetimi
- `GET /api/users` - Tüm kullanıcıları listele
- `GET /api/users/{id}` - Kullanıcı detayı
- `POST /api/users` - Yeni kullanıcı oluştur
- `PUT /api/users/{id}` - Kullanıcı güncelle
- `DELETE /api/users/{id}` - Kullanıcı sil

### İçerik Yönetimi
- `GET /api/contents` - İçerikleri listele (filtreleme destekli)
- `GET /api/contents/{id}` - İçerik detayı
- `GET /api/contents/{id}/variants` - İçerik varyantları
- `POST /api/contents` - Yeni içerik oluştur
- `PUT /api/contents/{id}` - İçerik güncelle
- `DELETE /api/contents/{id}` - İçerik sil

### Kategori Yönetimi
- `GET /api/categories` - Kategorileri listele
- `POST /api/categories` - Yeni kategori oluştur

## Cache Stratejisi

### Cache Politikaları
- **Kullanıcı Verileri**: 15 dakika cache süresi
- **İçerik Verileri**: 15 dakika cache süresi
- **Kategori Verileri**: 30 dakika cache süresi

### Cache Keys
```
users:{userId}
contents:{contentId}
contents:category:{categoryId}
contents:language:{language}
```

## Varyant Yönetimi

Sistem, stateful varyant yönetimi sağlar:
- Bir kullanıcı bir içerik varyantını gördüğünde, aynı varyant sonraki isteklerde de sunulur
- Her içerik minimum 2 varyanta sahiptir
- Varyant seçimi kullanıcı bazında cookie/session ile yönetilir

## Test Etme

### Unit Tests
```bash
dotnet test tests/NetCoreCase.UnitTests
```

### Integration Tests
```bash
dotnet test tests/NetCoreCase.IntegrationTests
```

## Konfigürasyon

### Cache Ayarları
```json
{
  "CacheSettings": {
    "DefaultExpirationMinutes": 15,
    "UseRedis": false,
    "RedisConnectionString": "localhost:6379"
  }
}
```

### Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## Destek

Herhangi bir sorun yaşarsanız veya önerileriniz varsa, lütfen [issue](../../issues) oluşturun.

## Roadmap

### Gelecek Özellikler
- [ ] GraphQL API desteği
- [ ] Elasticsearch entegrasyonu
- [ ] Real-time bildirimler
- [ ] Advanced admin paneli
- [ ] Mobile API endpoints
- [ ] Content versioning
- [ ] Advanced caching strategies

## Performans Metrikleri

- **API Response Time**: < 200ms (cached)
- **Database Query Time**: < 100ms
- **Cache Hit Ratio**: > 80%
- **Concurrent Users**: 1000+

---

**Not**: Bu README, projenin gelişim sürecinde güncellenecektir. En güncel bilgiler için bu dosyayı takip edin. 