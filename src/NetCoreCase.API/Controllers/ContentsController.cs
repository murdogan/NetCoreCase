using Microsoft.AspNetCore.Mvc;
using NetCoreCase.Application.DTOs.Content;
using NetCoreCase.Application.Interfaces;

namespace NetCoreCase.API.Controllers;

/// <summary>
/// İçerik yönetimi endpoints'leri
/// </summary>
public class ContentsController : BaseController
{
    private readonly IContentService _contentService;
    private readonly ILogger<ContentsController> _logger;

    public ContentsController(IContentService contentService, ILogger<ContentsController> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm içerikleri listeler
    /// </summary>
    /// <returns>İçerik listesi</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetAllContents(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tüm içerikler listeleniyor");
        
        var contents = await _contentService.GetAllAsync(cancellationToken);
        return SuccessResult(contents, "İçerikler başarıyla listelendi.");
    }

    /// <summary>
    /// ID'ye göre içerik getirir
    /// </summary>
    /// <param name="id">İçerik ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>İçerik bilgileri</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İçerik getiriliyor: {ContentId}", id);
        
        var content = await _contentService.GetByIdAsync(id, cancellationToken);
        
        if (content == null)
            return NotFoundResult($"ID'si {id} olan içerik bulunamadı.");

        return SuccessResult(content, "İçerik başarıyla getirildi.");
    }

    /// <summary>
    /// Kullanıcıya göre içerikleri listeler
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcının içerikleri</returns>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentsByUserId(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcıya göre içerikler getiriliyor: {UserId}", userId);
        
        var contents = await _contentService.GetByUserIdAsync(userId, cancellationToken);
        return SuccessResult(contents, "Kullanıcı içerikleri başarıyla getirildi.");
    }

    /// <summary>
    /// Kategoriye göre içerikleri listeler
    /// </summary>
    /// <param name="categoryId">Kategori ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kategorinin içerikleri</returns>
    [HttpGet("by-category/{categoryId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentsByCategoryId(Guid categoryId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategoriye göre içerikler getiriliyor: {CategoryId}", categoryId);
        
        var contents = await _contentService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return SuccessResult(contents, "Kategori içerikleri başarıyla getirildi.");
    }

    /// <summary>
    /// Dile göre içerikleri listeler
    /// </summary>
    /// <param name="language">Dil kodu (tr/en)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dildeki içerikler</returns>
    [HttpGet("by-language/{language}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentsByLanguage(string language, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dile göre içerikler getiriliyor: {Language}", language);
        
        try
        {
            var contents = await _contentService.GetByLanguageAsync(language, cancellationToken);
            return SuccessResult(contents, $"'{language}' dilindeki içerikler başarıyla getirildi.");
        }
        catch (ArgumentException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Kategori ve dile göre içerikleri listeler
    /// </summary>
    /// <param name="categoryId">Kategori ID'si</param>
    /// <param name="language">Dil kodu (tr/en)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Filtrelenmiş içerikler</returns>
    [HttpGet("by-category/{categoryId:guid}/language/{language}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentsByCategoryAndLanguage(Guid categoryId, string language, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kategori ve dile göre içerikler getiriliyor: {CategoryId}, {Language}", categoryId, language);
        
        try
        {
            var contents = await _contentService.GetByCategoryAndLanguageAsync(categoryId, language, cancellationToken);
            return SuccessResult(contents, "Filtrelenmiş içerikler başarıyla getirildi.");
        }
        catch (ArgumentException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// İçeriği varyantlarıyla birlikte getirir
    /// </summary>
    /// <param name="id">İçerik ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>İçerik ve varyantları</returns>
    [HttpGet("{id:guid}/with-variants")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentWithVariants(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İçerik varyantlarla birlikte getiriliyor: {ContentId}", id);
        
        var content = await _contentService.GetWithVariantsAsync(id, cancellationToken);
        
        if (content == null)
            return NotFoundResult($"ID'si {id} olan içerik bulunamadı.");

        return SuccessResult(content, "İçerik varyantlarla birlikte getirildi.");
    }

    /// <summary>
    /// İçerik arama
    /// </summary>
    /// <param name="searchTerm">Arama terimi</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Arama sonuçları</returns>
    [HttpGet("search")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> SearchContents([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İçerik araması yapılıyor: {SearchTerm}", searchTerm);
        
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequestResult("Arama terimi boş olamaz.");

        var contents = await _contentService.SearchAsync(searchTerm, cancellationToken);
        return SuccessResult(contents, $"'{searchTerm}' için arama sonuçları getirildi.");
    }

    /// <summary>
    /// Yeni içerik oluşturur
    /// </summary>
    /// <param name="createContentDto">İçerik oluşturma bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan içerik</returns>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> CreateContent([FromBody] CreateContentDto createContentDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Yeni içerik oluşturuluyor: {ContentTitle}", createContentDto.Title);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz içerik bilgileri.");

        try
        {
            var content = await _contentService.CreateAsync(createContentDto, cancellationToken);
            return CreatedResult(content, "İçerik başarıyla oluşturuldu.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// İçerik günceller
    /// </summary>
    /// <param name="id">Güncellenecek içerik ID'si</param>
    /// <param name="updateContentDto">Güncelleme bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Güncellenmiş içerik</returns>
    /// <remarks>
    /// Bu endpoint mevcut bir içeriği günceller. Tüm alanları sağlamak zorundasınız.
    /// Kategoryi değiştirmek mümkündür ancak kategori ID'si geçerli olmalıdır.
    /// </remarks>
    /// <response code="200">İçerik başarıyla güncellendi</response>
    /// <response code="400">Geçersiz veri</response>
    /// <response code="404">İçerik bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> UpdateContent(Guid id, [FromBody] UpdateContentDto updateContentDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İçerik güncelleniyor: {ContentId}", id);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz içerik bilgileri.");

        try
        {
            var content = await _contentService.UpdateAsync(id, updateContentDto, cancellationToken);
            return SuccessResult(content, "İçerik başarıyla güncellendi.");
        }
        catch (InvalidOperationException ex)
        {
            return NotFoundResult(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// İçerik siler
    /// </summary>
    /// <param name="id">Silinecek içerik ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Silme sonucu</returns>
    /// <remarks>
    /// Bu endpoint bir içeriği ve tüm varyantlarını tamamen siler. 
    /// Bu işlem geri alınamaz!
    /// </remarks>
    /// <response code="200">İçerik başarıyla silindi</response>
    /// <response code="404">İçerik bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteContent(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İçerik siliniyor: {ContentId}", id);
        
        var result = await _contentService.DeleteAsync(id, cancellationToken);
        
        if (!result)
            return NotFoundResult($"ID'si {id} olan içerik bulunamadı.");

        return SuccessResult("İçerik başarıyla silindi.");
    }

    #region Variant Management

    /// <summary>
    /// İçeriğin varsayılan varyantını getirir
    /// </summary>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Varsayılan varyant</returns>
    /// <remarks>
    /// Her içeriğin bir varsayılan varyantı vardır. Bu endpoint o varyantı döndürür.
    /// </remarks>
    /// <response code="200">Varsayılan varyant bulundu</response>
    /// <response code="404">İçerik veya varyant bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{contentId:guid}/variants/default")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetDefaultVariant(Guid contentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Default varyant getiriliyor: {ContentId}", contentId);
        
        var variant = await _contentService.GetDefaultVariantAsync(contentId, cancellationToken);
        
        if (variant == null)
            return NotFoundResult($"ID'si {contentId} olan içerik için default varyant bulunamadı.");

        return SuccessResult(variant, "Default varyant başarıyla getirildi.");
    }

    /// <summary>
    /// Kullanıcıya özel varyant getirir (Stateful)
    /// </summary>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcıya özel varyant</returns>
    /// <remarks>
    /// Bu endpoint kullanıcının daha önce gördüğü varyantı döndürür. 
    /// Eğer daha önce görmemişse, rastgele bir varyant atar ve bir sonraki istekte aynısını döndürür.
    /// Bu özellik A/B testing için kullanılır.
    /// </remarks>
    /// <response code="200">Kullanıcıya özel varyant bulundu</response>
    /// <response code="404">İçerik veya varyant bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{contentId:guid}/variants/user/{userId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetUserVariant(Guid contentId, Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı varyantı getiriliyor: {ContentId}, {UserId}", contentId, userId);
        
        var variant = await _contentService.GetUserVariantAsync(contentId, userId, cancellationToken);
        
        if (variant == null)
            return NotFoundResult($"İçerik {contentId} için kullanıcı {userId} varyantı bulunamadı.");

        return SuccessResult(variant, "Kullanıcı varyantı başarıyla getirildi.");
    }

    /// <summary>
    /// İçeriğe yeni varyant ekler
    /// </summary>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="createVariantDto">Varyant oluşturma bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan varyant</returns>
    /// <remarks>
    /// Bu endpoint mevcut bir içeriğe yeni varyant ekler. 
    /// İsDefault true olarak işaretlenirse, önceki varsayılan varyant değiştirilir.
    /// </remarks>
    /// <response code="201">Varyant başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPost("{contentId:guid}/variants")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> AddVariant(Guid contentId, [FromBody] CreateContentVariantDto createVariantDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("İçeriğe varyant ekleniyor: {ContentId}", contentId);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz varyant bilgileri.");

        try
        {
            var variant = await _contentService.AddVariantAsync(contentId, createVariantDto, cancellationToken);
            return CreatedResult($"/api/contents/{contentId}/variants/{variant.Id}", variant, "Varyant başarıyla oluşturuldu.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Varyantı varsayılan olarak işaretler
    /// </summary>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="variantId">Varyant ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>İşlem sonucu</returns>
    /// <remarks>
    /// Bu endpoint belirtilen varyantı varsayılan varyant yapar. 
    /// Önceki varsayılan varyant otomatik olarak normal varyant haline gelir.
    /// </remarks>
    /// <response code="200">Varyant varsayılan olarak işaretlendi</response>
    /// <response code="400">Geçersiz veri</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPut("{contentId:guid}/variants/{variantId:guid}/set-default")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> SetDefaultVariant(Guid contentId, Guid variantId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Default varyant ayarlanıyor: {ContentId}, {VariantId}", contentId, variantId);
        
        try
        {
            await _contentService.SetDefaultVariantAsync(contentId, variantId, cancellationToken);
            return SuccessResult("Varyant başarıyla default olarak ayarlandı.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    #endregion

    #region Stateful Content Management

    /// <summary>
    /// Kullanıcıya özel içerik getirir (Stateful varyant yönetimi ile)
    /// </summary>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcıya özel içerik ve varyant</returns>
    /// <remarks>
    /// Bu endpoint içeriği kullanıcıya özel varyantıyla birlikte döndürür.
    /// Kullanıcı daha önce bu içeriği görmüşse aynı varyantı, görmemişse yeni bir varyant atar.
    /// Her istekte kullanıcının görüntüleme geçmişi güncellenir.
    /// </remarks>
    /// <response code="200">İçerik ve kullanıcıya özel varyant</response>
    /// <response code="404">İçerik bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{contentId:guid}/for-user/{userId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetContentForUser(Guid contentId, Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcıya özel içerik getiriliyor: {ContentId}, {UserId}", contentId, userId);
        
        var content = await _contentService.GetContentForUserAsync(contentId, userId, cancellationToken);
        
        if (content == null)
            return NotFoundResult($"ID'si {contentId} olan içerik bulunamadı.");

        return SuccessResult(content, "Kullanıcıya özel içerik başarıyla getirildi.");
    }

    /// <summary>
    /// Kullanıcının belirli içerik için görüntüleme geçmişini getirir
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <param name="contentId">İçerik ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcının bu içerik için görüntüleme geçmişi</returns>
    /// <remarks>
    /// Bu endpoint kullanıcının belirli bir içerik için görüntüleme istatistiklerini döndürür.
    /// Hangi varyantı gördüğü, kaç kez eriştiği ve son erişim zamanı gibi bilgileri içerir.
    /// </remarks>
    /// <response code="200">Görüntüleme geçmişi bulundu</response>
    /// <response code="404">Geçmiş bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("users/{userId:guid}/content-history/{contentId:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetUserVariantHistory(Guid userId, Guid contentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı varyant geçmişi getiriliyor: {UserId}, {ContentId}", userId, contentId);
        
        var history = await _contentService.GetUserVariantHistoryAsync(userId, contentId, cancellationToken);
        
        if (history == null)
            return NotFoundResult($"Kullanıcı {userId} için içerik {contentId} geçmişi bulunamadı.");

        return SuccessResult(history, "Kullanıcı varyant geçmişi başarıyla getirildi.");
    }

    /// <summary>
    /// Kullanıcının tüm içerik görüntüleme geçmişini getirir
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcının tüm görüntüleme geçmişi</returns>
    /// <remarks>
    /// Bu endpoint kullanıcının tüm içerikler için görüntüleme geçmişini döndürür.
    /// Hangi içerikleri hangi varyantlarla gördüğü, son erişim zamanları ve görüntüleme sayıları yer alır.
    /// Sonuçlar son erişim zamanına göre sıralanır.
    /// </remarks>
    /// <response code="200">Kullanıcının görüntüleme geçmişi</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("users/{userId:guid}/view-history")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetUserViewHistory(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı görüntüleme geçmişi getiriliyor: {UserId}", userId);
        
        var history = await _contentService.GetUserViewHistoryAsync(userId, cancellationToken);
        return SuccessResult(history, "Kullanıcı görüntüleme geçmişi başarıyla getirildi.");
    }

    #endregion
} 