using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Services.Interfaces;
public interface IBankService
{
    Task<ModelBank> Get(string bankId);
    Task<List<ModelBank>> Gets();
    Task<ModelBank> Post(BankPostDto model);
    Task<ModelBank> Patch(string bankId, BankPatchDto model);
    Task<bool> Delete(string bankId);
    // List
    Task<DeleteBanksResult> Deletes(List<string> Ids);
    Task<UpsertBanksResult> Upserts(List<BankUpsertDto> models); //Update - Insert
}
