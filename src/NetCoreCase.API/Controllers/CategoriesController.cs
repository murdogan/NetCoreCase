using Microsoft.AspNetCore.Mvc;
using NetCoreCase.Application.DTOs.Category;
using NetCoreCase.Application.Interfaces;

namespace NetCoreCase.API.Controllers;

/// <summary>
/// Kategori yönetimi endpoints'leri
/// </summary>
public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm kategorileri listeler
    /// </summary>
    /// <returns>Kategori listesi</returns>
    /// <remarks>
    /// Sistemdeki tüm kategorileri içerik sayılarıyla birlikte döndürür.
    /// Kategoriler alfabetik sıraya göre listelenir.
    /// </remarks>
    /// <response code="200">Kategoriler başarıyla listelendi</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetAllCategories(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tüm kategoriler listeleniyor");
        
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        return SuccessResult(categories, "Kategoriler başarıyla listelendi.");
    }

    /// <summary>
    /// ID'ye göre kategori getirir
    /// </summary>
    /// <param name="id">Kategori ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kategori bilgileri</returns>
    /// <remarks>
    /// Belirtilen ID'ye sahip kategoriyi tüm bilgileriyle birlikte döndürür.
    /// Kategori bulunamazsa 404 hatası döner.
    /// </remarks>
    /// <response code="200">Kategori başarıyla bulundu</response>
    /// <response code="404">Kategori bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategori getiriliyor: {CategoryId}", id);
        
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        
        if (category == null)
            return NotFoundResult($"ID'si {id} olan kategori bulunamadı.");

        return SuccessResult(category, "Kategori başarıyla getirildi.");
    }

    /// <summary>
    /// İsme göre kategori getirir
    /// </summary>
    /// <param name="name">Kategori ismi</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kategori bilgileri</returns>
    /// <remarks>
    /// Kategori ismini kullanarak arama yapar. Arama büyük/küçük harf duyarsızdır.
    /// Bu endpoint kategori isminin unique olmasından dolayı tekil sonuç döndürür.
    /// </remarks>
    /// <response code="200">Kategori başarıyla bulundu</response>
    /// <response code="404">Kategori bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetCategoryByName(string name, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İsim ile kategori getiriliyor: {CategoryName}", name);
        
        var category = await _categoryService.GetByNameAsync(name, cancellationToken);
        
        if (category == null)
            return NotFoundResult($"İsmi '{name}' olan kategori bulunamadı.");

        return SuccessResult(category, "Kategori başarıyla getirildi.");
    }

    /// <summary>
    /// Kategoriyi içerikleriyle birlikte getirir
    /// </summary>
    /// <param name="id">Kategori ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kategori ve içerikleri</returns>
    /// <remarks>
    /// Kategoriyi ve bu kategoriye ait tüm içerikleri döndürür.
    /// İçerikler oluşturulma tarihine göre azalan sırada listelenir.
    /// Her içerik kullanıcı ve varyant bilgileriyle birlikte gelir.
    /// </remarks>
    /// <response code="200">Kategori ve içerikler başarıyla bulundu</response>
    /// <response code="404">Kategori bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{id:guid}/with-contents")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetCategoryWithContents(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategori içeriklerle birlikte getiriliyor: {CategoryId}", id);
        
        var category = await _categoryService.GetWithContentsAsync(id, cancellationToken);
        
        if (category == null)
            return NotFoundResult($"ID'si {id} olan kategori bulunamadı.");

        return SuccessResult(category, "Kategori içeriklerle birlikte getirildi.");
    }

    /// <summary>
    /// Yeni kategori oluşturur
    /// </summary>
    /// <param name="createCategoryDto">Kategori oluşturma bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan kategori</returns>
    /// <remarks>
    /// Yeni bir kategori oluşturur. Kategori ismi unique olmalıdır.
    /// Aynı isimde kategori mevcutsa hata döner.
    /// </remarks>
    /// <response code="201">Kategori başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri veya isim çakışması</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Yeni kategori oluşturuluyor: {CategoryName}", createCategoryDto.Name);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz kategori bilgileri.");

        try
        {
            var category = await _categoryService.CreateAsync(createCategoryDto, cancellationToken);
            return CreatedResult(category, "Kategori başarıyla oluşturuldu.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Kategori bilgilerini günceller
    /// </summary>
    /// <param name="id">Güncellenecek kategori ID'si</param>
    /// <param name="updateCategoryDto">Güncelleme bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Güncellenmiş kategori</returns>
    /// <remarks>
    /// Mevcut kategoriyi günceller. Yeni isim unique olmalıdır.
    /// Kategori bulunamazsa 404, isim çakışması varsa 400 hatası döner.
    /// </remarks>
    /// <response code="200">Kategori başarıyla güncellendi</response>
    /// <response code="400">Geçersiz veri veya isim çakışması</response>
    /// <response code="404">Kategori bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategori güncelleniyor: {CategoryId}", id);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz kategori bilgileri.");

        try
        {
            var category = await _categoryService.UpdateAsync(id, updateCategoryDto, cancellationToken);
            return SuccessResult(category, "Kategori başarıyla güncellendi.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Kategori siler
    /// </summary>
    /// <param name="id">Silinecek kategori ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Silme sonucu</returns>
    /// <remarks>
    /// Kategoriyi siler. Kategori silinemez eğer:
    /// - Kategoriye ait içerikler varsa
    /// - Kategori bulunamazsa
    /// Bu durumda uygun hata mesajı döner.
    /// </remarks>
    /// <response code="200">Kategori başarıyla silindi</response>
    /// <response code="400">Kategori silinemez (içerikler mevcut)</response>
    /// <response code="404">Kategori bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategori siliniyor: {CategoryId}", id);
        
        try
        {
            var result = await _categoryService.DeleteAsync(id, cancellationToken);
            
            if (!result)
                return NotFoundResult($"ID'si {id} olan kategori bulunamadı.");

            return SuccessResult("Kategori başarıyla silindi.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Kategori adının kullanımda olup olmadığını kontrol eder
    /// </summary>
    /// <param name="name">Kontrol edilecek kategori adı</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanım durumu</returns>
    /// <remarks>
    /// Bu endpoint kategori oluşturmadan önce isim çakışması kontrolü için kullanılır.
    /// Response'da exists: true/false şeklinde bilgi döner.
    /// Arama büyük/küçük harf duyarsızdır.
    /// </remarks>
    /// <response code="200">Kontrol sonucu döndürüldü</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("check-name/{name}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> CheckNameExists(string name, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategori adı kontrolü yapılıyor: {CategoryName}", name);
        
        var exists = await _categoryService.NameExistsAsync(name, cancellationToken);
        var message = exists ? "Kategori adı kullanımda." : "Kategori adı kullanılabilir.";
        
        return SuccessResult(exists, message);
    }
} 