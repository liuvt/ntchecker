using TaxiNT.Libraries.Models;

namespace TaxiNT.Services.Interfaces
{
    public interface IBlogServer
    {
        Task<IEnumerable<Blog>> GetAllAsync();
        Task<Blog?> GetByIdAsync(int id);
        Task<Blog?> GetBySlugAsync(string slug);
        Task<IEnumerable<Blog>> GetRelatedAsync(string? category, int excludeId, int take = 4);
        Task<IEnumerable<Blog>> GetMostPopularAsync(int take = 5);
        Task<Blog> CreateAsync(Blog blog);
        Task<bool> UpdateAsync(Blog blog);
        Task<bool> DeleteAsync(int id);
        Task IncrementViewCountAsync(int id);
    }
}
