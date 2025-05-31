namespace NetCoreCase.Domain.Entities;

public class UserContentVariantHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ContentId { get; set; }
    public Guid VariantId { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; } = 1;
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Content Content { get; set; } = null!;
    public virtual ContentVariant Variant { get; set; } = null!;
} 