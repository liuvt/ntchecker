
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
                //kilometer = Convert.ToInt32(item.ltvGetValueDecimal(3)),
                //kilometerWithCustomer = Convert.ToInt32(item.ltvGetValueDecimal(4)),
                businessDays = Convert.ToInt32(item.ltvGetValueDecimal(3)),
                salaryBase = item.ltvGetValueString(4),
                deductForDeposit = item.ltvGetValueString(5),//Trừ ký quỹ 
                deductForAccident = item.ltvGetValueString(6),//Trừ tai nạn
                deductForSalaryAdvance = item.ltvGetValueString(7),//Trừ lương ứng
                deductForViolationReport = item.ltvGetValueString(8),//Trừ vi phạm biên bản
                deductForSocialInsurance = item.ltvGetValueString(9),//Trừ BHXH
                deductForPIT = item.ltvGetValueString(10),//Trừ TNCN - Personal Income Tax Deduction 
                deductForVMV = item.ltvGetValueString(11),//Lỗi bảo quản xe: Vehicle Maintenance Violation
                deductForUV = item.ltvGetValueString(12),//Lỗi đồng phục: Uniform Violation
                deductForSHV = item.ltvGetValueString(13),//Lỗi giao ca: Shift Handover Violation
                deductForChargingPenalty = item.ltvGetValueString(14),// Phạt sạt: Charging Penalty
                deductForCharging = item.ltvGetValueString(15),//Sạt pin
                deductForTollPayment = item.ltvGetValueString(16), //Trừ tiền qua trạm : Deduction for Toll Payment
                deductForOrder = item.ltvGetValueString(17),//Trừ khác
                noteDeductOrder = item.ltvGetValueString(18),//Ghi chú trừ khác
                deductForOrderSalaryAdvance = item.ltvGetValueString(19),//Trừ tạm ứng: nợ doanh thu, hoặc ứng tiền vì mục đích nào đó, kế toán cho phép
                deductForNegativeSalary = item.ltvGetValueString(20),//Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương
                no_sua_chua = item.ltvGetValueString(21),//Nợ sửa chữa
                deductTotal = item.ltvGetValueString(22), //Tổng trừ
                salaryNet = item.ltvGetValueString(23),//Lương thực nhận
                salaryDate = item.ltvGetValueString(24),//Tháng/năm
                salaryType = item.ltvGetValueString(25),//Loại hình lương
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
