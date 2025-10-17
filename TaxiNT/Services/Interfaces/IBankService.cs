using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Services.Interfaces;
public interface IBankService
{
    Task<Bank> Get(string bankId);
    Task<List<Bank>> Gets();
    Task<Bank> Post(BankPostDto model);
    Task<Bank> Patch(string bankId, BankPatchDto model);
    Task<bool> Delete(string bankId);
    // List
    Task<DeleteBanksResult> Deletes(List<string> Ids);
    Task<UpsertBanksResult> Upserts(List<BankUpsertDto> models); //Update - Insert
}
