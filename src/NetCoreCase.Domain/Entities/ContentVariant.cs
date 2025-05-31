namespace NetCoreCase.Domain.Entities;

public class ContentVariant : BaseEntity
{
    public Guid ContentId { get; set; }
    public string VariantData { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    
    // Navigation Properties
    public virtual Content Content { get; set; } = null!;
} 