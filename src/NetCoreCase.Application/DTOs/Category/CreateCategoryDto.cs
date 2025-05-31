using System.ComponentModel.DataAnnotations;

namespace NetCoreCase.Application.DTOs.Category;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Kategori adı 2-100 karakter arasında olmalıdır.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    public string Description { get; set; } = string.Empty;
} 