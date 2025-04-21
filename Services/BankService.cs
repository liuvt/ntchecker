using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ntchecker.Data.Models.GGSheetModels;
using ntchecker.Extensions;
using ntchecker.Services.Interfaces;

namespace ntchecker.Services;
public class BankService : IBankService
{
    #region Constructor 
    //For Connection to Spread
    private SheetsService sheetsService;
    private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private readonly string CredentialGGSheetService = "ggsheetaccount.json";
    private readonly string AppName = "NTBL Taxi";
    private readonly string SpreadSheetId = "1dISl39trOtqlH-oTltYnbCiQec3VA12dCebysh5KyM8";
    private readonly string SpreadSheetIdHistory = "1adyPqtzm112pV1LbegUXYyzEzfM36KbNL-mamMpOX-8";

    // For Sheet
    private readonly string sheetBANK = "BANK";

    public BankService()
    {
        //File xác thực google tài khoản
        GoogleCredential credential;
        using (var stream = new FileStream(CredentialGGSheetService, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
        }

        // Đăng ký service
        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = AppName,
        });
    }
    #endregion

    #region Bank
    // Lấy toàn bộ danh sách
    private async Task<List<Bank>> GetsBank()
    {
        var dts = new List<Bank>();
        var range = $"{sheetBANK}!A2:I";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {

            dts.Add(new Bank
            {
                bank_Id = item.ltvGetValue(0),
                bank_NumberId = item.ltvGetValue(1),
                bank_Name = item.ltvGetValue(2),
                bank_Number = item.ltvGetValue(3),
                bank_Type = item.ltvGetValue(4),
                bank_AccountName = item.ltvGetValue(5),
                bank_Url = item.ltvGetValue(6),
                bank_Status = item.ltvGetValue(7),
                createdAt = item.ltvGetValue(8),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã bankId 
    public async Task<Data.Models.GGSheetModels.Bank> GetBank(string bankId)
    {
        var dts = await GetsBank() ?? new List<Bank>();
        return dts.FirstOrDefault(e => e.bank_Id.Equals(bankId, StringComparison.OrdinalIgnoreCase)) ?? new Bank();
    }
    #endregion

    #region Bank theo _sheetBANK
    // Lấy toàn bộ danh sách theo _sheetBANK
    private async Task<List<Bank>> GetsBank(string _SpreadSheetId, string _sheetBANK)
    {
        var dts = new List<Bank>();
        var range = $"{_sheetBANK}!A2:I";
        var values = await sheetsService.ltvGetSheetValuesAsync(_SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {

            dts.Add(new Bank
            {
                bank_Id = item.ltvGetValue(0),
                bank_NumberId = item.ltvGetValue(1),
                bank_Name = item.ltvGetValue(2),
                bank_Number = item.ltvGetValue(3),
                bank_Type = item.ltvGetValue(4),
                bank_AccountName = item.ltvGetValue(5),
                bank_Url = item.ltvGetValue(6),
                bank_Status = item.ltvGetValue(7),
                createdAt = item.ltvGetValue(8),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã bankId 
    public async Task<Data.Models.GGSheetModels.Bank> GetBank(string _SpreadSheetId, string _sheetBANK, string bankId)
    {
        var dts = await GetsBank(_SpreadSheetId, _sheetBANK) ?? new List<Bank>();
        return dts.FirstOrDefault(e => e.bank_Id.Equals(bankId, StringComparison.OrdinalIgnoreCase)) ?? new Bank();
    }
    #endregion
}
