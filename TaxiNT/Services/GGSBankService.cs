using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;
using TaxiNT.Data;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Models.GGSheets;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services;
public class GGSBankService : IGGSBankService
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

    public GGSBankService()
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
                bank_Id = item.ltvGetValueString(0),
                bank_NumberId = item.ltvGetValueString(1),
                bank_Name = item.ltvGetValueString(2),
                bank_Number = item.ltvGetValueString(3),
                bank_Type = item.ltvGetValueString(4),
                bank_AccountName = item.ltvGetValueString(5),
                bank_Url = item.ltvGetValueString(6),
                bank_Status = item.ltvGetValueString(7),
                createdAt = item.ltvGetValueString(8),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã bankId 
    public async Task<Bank> GetBank(string bankId)
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
                bank_Id = item.ltvGetValueString(0),
                bank_NumberId = item.ltvGetValueString(1),
                bank_Name = item.ltvGetValueString(2),
                bank_Number = item.ltvGetValueString(3),
                bank_Type = item.ltvGetValueString(4),
                bank_AccountName = item.ltvGetValueString(5),
                bank_Url = item.ltvGetValueString(6),
                bank_Status = item.ltvGetValueString(7),
                createdAt = item.ltvGetValueString(8),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã bankId 
    public async Task<Bank> GetBank(string _SpreadSheetId, string _sheetBANK, string bankId)
    {
        var dts = await GetsBank(_SpreadSheetId, _sheetBANK) ?? new List<Bank>();
        return dts.FirstOrDefault(e => e.bank_Id.Equals(bankId, StringComparison.OrdinalIgnoreCase)) ?? new Bank();
    }
    #endregion


}
