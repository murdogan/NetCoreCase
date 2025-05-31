using NetCoreCase.Domain.Entities;

namespace NetCoreCase.Domain.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Category?> GetWithContentsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoriesWithContentCountAsync(CancellationToken cancellationToken = default);
} 