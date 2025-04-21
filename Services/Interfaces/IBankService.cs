using ntchecker.Data.Models.GGSheetModels;

namespace ntchecker.Services.Interfaces;
public interface IBankService
{
    Task<Data.Models.GGSheetModels.Bank> GetBank(string bankId);
    Task<Data.Models.GGSheetModels.Bank> GetBank(string _SpreadSheetId, string _sheetBANK, string bankId);
}
