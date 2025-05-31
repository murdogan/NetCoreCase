namespace NetCoreCase.Domain.Entities;

public class Content : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty; // en, tr
    public string ImageUrl { get; set; } = string.Empty;
    
    // Foreign Keys
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<ContentVariant> Variants { get; set; } = new List<ContentVariant>();
} 