using System.ComponentModel.DataAnnotations;

namespace NetCoreCase.Application.DTOs.Content;

public class CreateContentVariantDto
{
    [Required(ErrorMessage = "Varyant verisi zorunludur.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Varyant verisi 10-2000 karakter arasında olmalıdır.")]
    public string VariantData { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
} 