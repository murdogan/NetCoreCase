using System.ComponentModel.DataAnnotations;

namespace NetCoreCase.Application.DTOs.Content;

public class UpdateContentDto
{
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Başlık 5-200 karakter arasında olmalıdır.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Açıklama 10-1000 karakter arasında olmalıdır.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Dil zorunludur.")]
    [RegularExpression("^(tr|en)$", ErrorMessage = "Dil 'tr' veya 'en' olmalıdır.")]
    public string Language { get; set; } = string.Empty;

    [Url(ErrorMessage = "Geçerli bir URL giriniz.")]
    [StringLength(500, ErrorMessage = "Görsel URL en fazla 500 karakter olabilir.")]
    public string ImageUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori ID zorunludur.")]
    public Guid CategoryId { get; set; }
} 