using Microsoft.AspNetCore.Mvc;
using NetCoreCase.Application.DTOs.User;
using NetCoreCase.Application.Interfaces;

namespace NetCoreCase.API.Controllers;

/// <summary>
/// Kullanıcı yönetimi endpoints'leri
/// </summary>
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm kullanıcıları listeler
    /// </summary>
    /// <returns>Kullanıcı listesi</returns>
    /// <remarks>
    /// Sistemdeki tüm kullanıcıları içerik sayılarıyla birlikte döndürür.
    /// Kullanıcılar tam adlarına göre alfabetik sıraya göre listelenir.
    /// Hassas bilgiler (şifre vs.) response'da yer almaz.
    /// </remarks>
    /// <response code="200">Kullanıcılar başarıyla listelendi</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tüm kullanıcılar listeleniyor");
        
        var users = await _userService.GetAllAsync(cancellationToken);
        return SuccessResult(users, "Kullanıcılar başarıyla listelendi.");
    }

    /// <summary>
    /// ID'ye göre kullanıcı getirir
    /// </summary>
    /// <param name="id">Kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcı bilgileri</returns>
    /// <remarks>
    /// Belirtilen ID'ye sahip kullanıcıyı tüm bilgileriyle birlikte döndürür.
    /// Hassas bilgiler (şifre vs.) response'da yer almaz.
    /// Kullanıcı bulunamazsa 404 hatası döner.
    /// </remarks>
    /// <response code="200">Kullanıcı başarıyla bulundu</response>
    /// <response code="404">Kullanıcı bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı getiriliyor: {UserId}", id);
        
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        
        if (user == null)
            return NotFoundResult($"ID'si {id} olan kullanıcı bulunamadı.");

        return SuccessResult(user, "Kullanıcı başarıyla getirildi.");
    }

    /// <summary>
    /// E-posta adresine göre kullanıcı getirir
    /// </summary>
    /// <param name="email">E-posta adresi</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcı bilgileri</returns>
    /// <remarks>
    /// E-posta adresini kullanarak kullanıcı arar. Arama büyük/küçük harf duyarsızdır.
    /// E-posta adresi unique olduğu için tekil sonuç döndürür.
    /// Kullanıcı bulunamazsa 404 hatası döner.
    /// </remarks>
    /// <response code="200">Kullanıcı başarıyla bulundu</response>
    /// <response code="404">Kullanıcı bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("E-posta ile kullanıcı getiriliyor: {Email}", email);
        
        var user = await _userService.GetByEmailAsync(email, cancellationToken);
        
        if (user == null)
            return NotFoundResult($"E-posta adresi {email} olan kullanıcı bulunamadı.");

        return SuccessResult(user, "Kullanıcı başarıyla getirildi.");
    }

    /// <summary>
    /// Kullanıcıyı içerikleriyle birlikte getirir
    /// </summary>
    /// <param name="id">Kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanıcı ve içerikleri</returns>
    /// <remarks>
    /// Kullanıcıyı ve bu kullanıcının oluşturduğu tüm içerikleri döndürür.
    /// İçerikler oluşturulma tarihine göre azalan sırada listelenir.
    /// Her içerik kategori ve varyant bilgileriyle birlikte gelir.
    /// </remarks>
    /// <response code="200">Kullanıcı ve içerikler başarıyla bulundu</response>
    /// <response code="404">Kullanıcı bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("{id:guid}/with-contents")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetUserWithContents(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı içeriklerle birlikte getiriliyor: {UserId}", id);
        
        var user = await _userService.GetWithContentsAsync(id, cancellationToken);
        
        if (user == null)
            return NotFoundResult($"ID'si {id} olan kullanıcı bulunamadı.");

        return SuccessResult(user, "Kullanıcı içeriklerle birlikte getirildi.");
    }

    /// <summary>
    /// Yeni kullanıcı oluşturur
    /// </summary>
    /// <param name="createUserDto">Kullanıcı oluşturma bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Oluşturulan kullanıcı</returns>
    /// <remarks>
    /// Yeni bir kullanıcı oluşturur. E-posta adresi unique olmalıdır.
    /// Aynı e-posta adresi ile kullanıcı mevcutsa hata döner.
    /// Şifre minimum 6 karakter olmalıdır.
    /// </remarks>
    /// <response code="201">Kullanıcı başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri veya e-posta çakışması</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Yeni kullanıcı oluşturuluyor: {Email}", createUserDto.Email);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz kullanıcı bilgileri.");

        try
        {
            var user = await _userService.CreateAsync(createUserDto, cancellationToken);
            return CreatedResult(user, "Kullanıcı başarıyla oluşturuldu.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Kullanıcı bilgilerini günceller
    /// </summary>
    /// <param name="id">Güncellenecek kullanıcı ID'si</param>
    /// <param name="updateUserDto">Güncelleme bilgileri</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Güncellenmiş kullanıcı</returns>
    /// <remarks>
    /// Mevcut kullanıcıyı günceller. Yeni e-posta adresi unique olmalıdır.
    /// Kullanıcı bulunamazsa 404, e-posta çakışması varsa 400 hatası döner.
    /// Şifre güncellenmek isteniyorsa minimum 6 karakter olmalıdır.
    /// </remarks>
    /// <response code="200">Kullanıcı başarıyla güncellendi</response>
    /// <response code="400">Geçersiz veri veya e-posta çakışması</response>
    /// <response code="404">Kullanıcı bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı güncelleniyor: {UserId}", id);
        
        if (!ModelState.IsValid)
            return BadRequestResult("Geçersiz kullanıcı bilgileri.");

        try
        {
            var user = await _userService.UpdateAsync(id, updateUserDto, cancellationToken);
            return SuccessResult(user, "Kullanıcı başarıyla güncellendi.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// Kullanıcı siler
    /// </summary>
    /// <param name="id">Silinecek kullanıcı ID'si</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Silme sonucu</returns>
    /// <remarks>
    /// Kullanıcıyı siler. Kullanıcı silinemez eğer:
    /// - Kullanıcıya ait içerikler varsa
    /// - Kullanıcı bulunamazsa
    /// Bu durumda uygun hata mesajı döner.
    /// </remarks>
    /// <response code="200">Kullanıcı başarıyla silindi</response>
    /// <response code="400">Kullanıcı silinemez (içerikler mevcut)</response>
    /// <response code="404">Kullanıcı bulunamadı</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kullanıcı siliniyor: {UserId}", id);
        
        try
        {
            var result = await _userService.DeleteAsync(id, cancellationToken);
            
            if (!result)
                return NotFoundResult($"ID'si {id} olan kullanıcı bulunamadı.");

            return SuccessResult("Kullanıcı başarıyla silindi.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequestResult(ex.Message);
        }
    }

    /// <summary>
    /// E-posta adresinin kullanımda olup olmadığını kontrol eder
    /// </summary>
    /// <param name="email">Kontrol edilecek e-posta adresi</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Kullanım durumu</returns>
    /// <remarks>
    /// Bu endpoint kullanıcı oluşturmadan önce e-posta çakışması kontrolü için kullanılır.
    /// Response'da exists: true/false şeklinde bilgi döner.
    /// Arama büyük/küçük harf duyarsızdır.
    /// </remarks>
    /// <response code="200">Kontrol sonucu döndürüldü</response>
    /// <response code="500">Sunucu hatası</response>
    [HttpGet("check-email/{email}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> CheckEmailExists(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("E-posta kontrolü yapılıyor: {Email}", email);
        
        var exists = await _userService.EmailExistsAsync(email, cancellationToken);
        var message = exists ? "E-posta adresi kullanımda." : "E-posta adresi kullanılabilir.";
        
        return SuccessResult(exists, message);
    }
} 