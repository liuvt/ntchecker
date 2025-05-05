namespace ntchecker.Services.Interfaces;

public interface IQuickLinkService
{
    Task Create(ntchecker.Data.Entities.QuickLinkDto quickLink);
    Task<List<ntchecker.Data.Models.QuickLink>> Gets();
    Task<ntchecker.Data.Models.QuickLink?> Get(string id);
    Task<bool> Update(ntchecker.Data.Models.QuickLink quickLink);
    Task<bool> Delete(string id);
}
