
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using TaxiNT.Libraries.Models.GGSheets;
using TaxiNT.Extensions;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services;

public class SalaryServer : ISalaryServer
{
    #region Constructor 
    //For Connection to Spread
    private SheetsService sheetsService;
    private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private readonly string CredentialGGSheetService = "ggsheetaccount.json";
    private readonly string AppName = "NTBL Taxi";
    private readonly string SpreadSheetId = "1adyPqtzm112pV1LbegUXYyzEzfM36KbNL-mamMpOX-8";

    // For Sheet
    private readonly string sheetSALARIES = "SALARIES";
    private readonly string sheetSALARYDETAILS = "SALARYDETAILS";

    public SalaryServer()
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

    #region Salary
    // Lấy toàn bộ danh sách
    private async Task<List<Salary>> Gets()
    {
        var dts = new List<Salary>();
        var range = $"{sheetSALARIES}!A2:Z";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {
            dts.Add(new Salary
            {
                userId = item.ltvGetValueString(0),
                revenue = item.ltvGetValueString(1),
                tripsTotal = Convert.ToInt32(item.ltvGetValueDecimal(2)),
                salaryType = item.ltvGetValueString(3), // Loại hình kinh doanh LƯƠNG NGÀY - LƯƠNG THÁNG
                businessDays = Convert.ToInt32(item.ltvGetValueDecimal(4)), // Số ngày KD
                salaryBase = item.ltvGetValueString(5), // Sau mức ăn chia || Lương cơ bản
                deductTotal = item.ltvGetValueString(6), //Tổng trừ
                salaryNet = item.ltvGetValueString(7),//Lương thực nhận

                deductForDeposit = item.ltvGetValueString(8), //1. Trừ ký quỹ 
                deductForSalaryAdvance = item.ltvGetValueString(9), //2. Trừ lương ứng
                deductForNegativeSalary = item.ltvGetValueString(10), //3. Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương (công ty)
                deductForViolationReport = item.ltvGetValueString(11), //4. Trừ vi phạm biên bản  
                no_sua_chua = item.ltvGetValueString(12), //5. Nợ sửa chữa
                haomon_voxe = item.ltvGetValueString(13), //6. Hao mòn vỏ xe
                deductForCharging = item.ltvGetValueString(14), //7. Sat pin
                deductForChargingPenalty = item.ltvGetValueString(15), //8. phạt sạt: Charging Penalty
                deductForTollPayment = item.ltvGetValueString(16), //9. Trừ tiền qua trạm : Deduction for Toll Payment
                deductForSocialInsurance = item.ltvGetValueString(17), //10. Trừ BHXH
                deductForNegativeSalaryPartner = item.ltvGetValueString(18), //11. Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương (Thương quyền)
                deductForPIT = item.ltvGetValueString(19), //12. Trừ TNCN - Personal Income Tax Deduction  
                deductForOrder = item.ltvGetValueString(20), //13. Trừ khác
                noteDeductOrder = item.ltvGetValueString(21), //14. Ghi chú trừ khác

                salaryDate = item.ltvGetValueString(22), // Tháng/năm
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã userId của tài xế [Họ tên - Mã nhân viên]
    // Gọi service Bank(_sheetBank) để add vào Revenue
    public async Task<Salary> GetSalary(string userId)
    {
        var dts = await Gets();
        var listSalary = dts.Where(e => e.userId.Equals(userId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        if (listSalary == null)
        {
            throw new Exception("Không tìm thấy dữ liệu: {userId}");
        }
        return listSalary;
    }
    #endregion

    #region Salary details
    private async Task<List<SalaryDetails>> GetsSalaryDetails()
    {
        var dts = new List<SalaryDetails>();
        var range = $"{sheetSALARYDETAILS}!A2:F";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {
            dts.Add(new SalaryDetails
            {
                userId = item.ltvGetValueString(0),
                revenue = item.ltvGetValueString(1),
                revenueAC = item.ltvGetValueString(2),
                type = item.ltvGetValueString(3),
                salaryBase = item.ltvGetValueString(4),
                daterevenues = item.ltvGetValueString(5),
                createdAt = item.ltvGetValueString(6),
            });
        }

        return dts;
    }

    public async Task<List<SalaryDetails>> GetSalaryDetails(string userId)
    {
        var dts = await GetsSalaryDetails();
        var listSalaryDetails = dts.Where(e => e.userId.Equals(userId, StringComparison.OrdinalIgnoreCase)).ToList();
        if (listSalaryDetails == null)
        {
            throw new Exception("Không tìm thấy dữ liệu: {userId}");
        }
        return listSalaryDetails;
    }
    #endregion
}
