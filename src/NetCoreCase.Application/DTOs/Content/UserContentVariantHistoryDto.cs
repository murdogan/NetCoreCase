namespace NetCoreCase.Application.DTOs.Content;

public class UserContentVariantHistoryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ContentId { get; set; }
    public Guid VariantId { get; set; }
    public DateTime ViewedAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation Properties
    public string UserFullName { get; set; } = string.Empty;
    public string ContentTitle { get; set; } = string.Empty;
    public string VariantData { get; set; } = string.Empty;
} 