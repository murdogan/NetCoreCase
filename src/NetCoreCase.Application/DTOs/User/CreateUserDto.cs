using System.ComponentModel.DataAnnotations;

namespace NetCoreCase.Application.DTOs.User;

public class CreateUserDto
{
    [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ad Soyad 2-100 karakter arasında olmalıdır.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(255, ErrorMessage = "E-posta en fazla 255 karakter olabilir.")]
    public string Email { get; set; } = string.Empty;
} 