using Microsoft.EntityFrameworkCore;
using ntchecker.Data;
using ntchecker.Data.Models;
using ntchecker.Data.Entities;

namespace ntchecker.Services;
public class QuickLinkService
{
    private readonly ntcheckerDBContext _dbContext;

    public QuickLinkService(ntcheckerDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Create(QuickLinkDto quickLink)
    {
        var models = new QuickLink
        {
            ql_Id = quickLink.ql_Id,
            ql_Url = quickLink.ql_Url,
            ql_BankId = quickLink.ql_BankId,
            ql_AccountNo = quickLink.ql_AccountNo,
            ql_template = quickLink.ql_template,
            ql_amount = quickLink.ql_amount,
            ql_description = quickLink.ql_description,
            ql_AccountName = quickLink.ql_AccountName,
        };

        _dbContext.QuickLinks.Add(models);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<QuickLink>> Gets() => await _dbContext.QuickLinks.ToListAsync();

    public async Task<QuickLink?> Get(string id) => await _dbContext.QuickLinks.FindAsync(id);

    public async Task<bool> Update(QuickLink quickLink)
    {
        var existing = await _dbContext.QuickLinks.FindAsync(quickLink.ql_Id);
        if (existing == null) return false;

        existing.ql_Url = quickLink.ql_Url;
        existing.ql_BankId = quickLink.ql_BankId;
        existing.ql_AccountNo = quickLink.ql_AccountNo;
        existing.ql_template = quickLink.ql_template;
        existing.ql_amount = quickLink.ql_amount;
        existing.ql_description = quickLink.ql_description;
        existing.ql_AccountName = quickLink.ql_AccountName;
        existing.Activity = quickLink.Activity;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(string id)
    {
        var quickLink = await _dbContext.QuickLinks.FindAsync(id);
        if (quickLink == null) return false;

        _dbContext.QuickLinks.Remove(quickLink);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
