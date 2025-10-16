using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using TaxiNT.Libraries.Models.GGSheets;
using TaxiNT.Extensions;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services;
public class OrderByHistoryService : IOrderByHistoryService
{
    #region Constructor 
    //For Connection to Spread
    private SheetsService sheetsService;
    private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private readonly string CredentialGGSheetService = "ggsheetaccount.json";
    private readonly string AppName = "NTBL Taxi";
    private readonly string SpreadSheetId = "1adyPqtzm112pV1LbegUXYyzEzfM36KbNL-mamMpOX-8";

    // For Sheet
    private readonly string sheetREVENUE = "LICHSUDANHSACHLENCA";
    private readonly string sheetDATALE = "LICHSUDATALE";
    private readonly string sheetDATAHOPDONG = "LICHSUDATAHOPDONG";
    private readonly string _sheetBank = "dbBanks";
    
    //Add service Bank
    private IGGSBankService bankService;

    public OrderByHistoryService(IGGSBankService _bankService)
    {
        this.bankService = _bankService;
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

    #region Revenues
    // Lấy toàn bộ danh sách
    private async Task<List<RevenueDetail>> GetsRevenueDetail()
    {
        var dts = new List<RevenueDetail>();
        var range = $"{sheetREVENUE}!A2:R";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {
            dts.Add(new RevenueDetail
            {
                numberCar = item.ltvGetValueString(0),
                userId = item.ltvGetValueString(1),
                revenueByMonth = item.ltvGetValueString(2),
                revenueByDate = item.ltvGetValueString(3),
                qrContext = item.ltvGetValueString(4),
                qrUrl = item.ltvGetValueString(5),
                discountOther = item.ltvGetValueString(6),
                arrearsOther = item.ltvGetValueString(7),
                totalPrice = item.ltvGetValueString(8),
                walletGSM = item.ltvGetValueString(9),
                discountGSM = item.ltvGetValueString(10),
                discountNT = item.ltvGetValueString(11),
                bank_Id = item.ltvGetValueString(12),
                createdAt = item.ltvGetValueString(13),
                typeCar = item.ltvGetValueString(14),
                Area = item.ltvGetValueString(15),
                Rank = item.ltvGetValueString(16),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã userId của tài xế [Họ tên - Mã nhân viên]
    public async Task<List<RevenueDetail>> GetsRevenueDetail(string userId)
    {
        var dts = await GetsRevenueDetail() ?? new List<RevenueDetail>();
        if (!string.IsNullOrWhiteSpace(userId))
        {
            dts = dts.Where(e => e.userId.Equals(userId, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return dts;
    }

    // Lọc lại danh sách theo mã userId của tài xế [Họ tên - Mã nhân viên]
    // Gọi service Bank(_sheetBank) để add vào Revenue
    public async Task<Revenue> GetRevenue(string userId, string date)
    {
        var dts = await GetsRevenueDetail();
        var listRevenue = dts
          .Where(e => e.userId.Equals(userId, StringComparison.OrdinalIgnoreCase))
          .Where(e =>
          {
              if (!string.IsNullOrWhiteSpace(e.createdAt) &&
                  DateTime.TryParseExact(e.createdAt, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
              {
                  return dt.ToString("ddMMyyyy") == date;
              }
              return false;
          })
          .ToList();

        if (!listRevenue.Any())
        {
            throw new Exception("Không tìm thấy dữ liệu: {userId}");
        }
        //Add dữ liệu bank chuyển khoản
        var _bank = await bankService.GetBank(SpreadSheetId, _sheetBank, listRevenue.First().bank_Id);
        var newRevenue = new Revenue
        {
            userId = userId,
            bank_Id = _bank.bank_Id,
            revenues = listRevenue,
            bank = _bank,
        };

        //Thiết lập lại Nội dung chuyển khoản và QR chuyển khoản
        var _qrContext = string.Join(" ", newRevenue.numberCar) + " " + newRevenue.userId.Replace("-", "").Replace(" ", "") + " " + newRevenue.createdAt;
        newRevenue.qrUrl = $@"{newRevenue.bank.bank_Url}{newRevenue.bank.bank_NumberId}-{newRevenue.bank.bank_Number}-{newRevenue.bank.bank_Type}?amount={newRevenue.totalPrice}&addInfo={_qrContext}&accountName={newRevenue.bank.bank_AccountName}";

        return newRevenue;
    }
    #endregion

    #region Timepieces
    // Lấy toàn bộ danh sách
    private async Task<List<TimepieceDetail>> GetsTimepieceDetail()
    {
        var dts = new List<TimepieceDetail>();
        var range = $"{sheetDATALE}!A2:J";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {

            dts.Add(new TimepieceDetail
            {
                numberCar = item.ltvGetValueString(0),
                tpTimeStart = item.ltvGetValueString(1),
                tpTimeEnd = item.ltvGetValueString(2),
                tpDistance = item.ltvGetValueString(3),
                tpPrice = item.ltvGetValueString(4).ltvVNDCurrency(),
                tpPickUp = item.ltvGetValueString(5),
                tpDropOut = item.ltvGetValueString(6),
                tpType = item.ltvGetValueString(7),
                userId = item.ltvGetValueString(8),
                createdAt = item.ltvGetValueString(9),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã userId của tài xế [Họ tên - Mã nhân viên]
    public async Task<Timepiece> GetTimepiece(string userId, string date)
    {
        var dts = await GetsTimepieceDetail() ?? new List<TimepieceDetail>();
        var listTimepiece = dts
           .Where(e => e.userId.Equals(userId, StringComparison.OrdinalIgnoreCase))
           .Where(e =>
           {
               if (!string.IsNullOrWhiteSpace(e.createdAt) &&
                   DateTime.TryParseExact(e.createdAt, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
               {
                   return dt.ToString("ddMMyyyy") == date;
               }
               return false;
           })
           .ToList();
        return new Timepiece
        {
            userId = userId,
            timepieces = listTimepiece
        };
    }
    #endregion

    #region Contracts

    // Lấy toàn bộ danh sách
    private async Task<List<GGSContractDetail>> GetsContractDetail()
    {
        var dts = new List<GGSContractDetail>();
        var range = $"{sheetDATAHOPDONG}!A2:K";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {

            dts.Add(new GGSContractDetail
            {
                ctId = item.ltvGetValueString(0),
                numberCar = item.ltvGetValueString(1),
                ctKey = item.ltvGetValueString(2),
                ctAmount = item.ltvGetValueString(3).ltvVNDCurrency(),
                ctDefaultDistance = item.ltvGetValueString(4),
                ctOverDistance = item.ltvGetValueString(5),
                ctSurcharge = item.ltvGetValueString(6).ltvVNDCurrency(),
                ctPromotion = item.ltvGetValueString(7).ltvVNDCurrency(),
                totalPrice = item.ltvGetValueString(8).ltvVNDCurrency(),
                userId = item.ltvGetValueString(9),
                createdAt = item.ltvGetValueString(10),
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã userId của tài xế [Họ tên - Mã nhân viên]
    public async Task<GGSContract> GetContract(string userId, string date)
    {
        var dts = await GetsContractDetail() ?? new List<GGSContractDetail>();

        var listContract = dts
            .Where(e => e.userId.Equals(userId, StringComparison.OrdinalIgnoreCase))
            .Where(e =>
            {
                if (!string.IsNullOrWhiteSpace(e.createdAt) &&
                    DateTime.TryParseExact(e.createdAt, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                {
                    return dt.ToString("ddMMyyyy") == date;
                }
                return false;
            })
            .ToList();

        return new GGSContract
        {
            userId = userId,
            contracts = listContract
        };
    }
    #endregion
}
