namespace NetCoreCase.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual ICollection<Content> Contents { get; set; } = new List<Content>();
} 