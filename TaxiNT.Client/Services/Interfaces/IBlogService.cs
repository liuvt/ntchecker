using TaxiNT.Libraries.Models;

namespace TaxiNT.Client.Services.Interfaces
{
    public interface IBlogService
    {
        Task<List<Blog>> GetAllAsync();
        Task<Blog?> GetByIdAsync(int id);
        Task<Blog?> GetBySlugAsync(string slug);
        Task<List<Blog>> GetRelatedAsync(string category, int excludeId);
        Task<List<Blog>> GetMostPopularAsync();
        Task<Blog?> CreateAsync(Blog blog);
        Task<bool> UpdateAsync(Blog blog);
        Task<bool> DeleteAsync(int id);
    }
}
