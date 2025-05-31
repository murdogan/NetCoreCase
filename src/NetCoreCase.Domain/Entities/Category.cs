namespace NetCoreCase.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
} 