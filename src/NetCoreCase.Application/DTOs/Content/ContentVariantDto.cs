namespace NetCoreCase.Application.DTOs.Content;

public class ContentVariantDto
{
    public Guid Id { get; set; }
    public Guid ContentId { get; set; }
    public string VariantData { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 