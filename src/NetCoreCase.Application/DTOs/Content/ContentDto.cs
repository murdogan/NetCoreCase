using NetCoreCase.Application.DTOs.Category;
using NetCoreCase.Application.DTOs.User;

namespace NetCoreCase.Application.DTOs.Content;

public class ContentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // İlişkili objeler
    public Guid UserId { get; set; }
    public UserDto? User { get; set; }
    
    public Guid CategoryId { get; set; }
    public CategoryDto? Category { get; set; }
    
    public int VariantCount { get; set; }
    public List<ContentVariantDto> Variants { get; set; } = new();
    
    // Stateful varyant yönetimi için
    public ContentVariantDto? UserSpecificVariant { get; set; }
} 