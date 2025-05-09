using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ntchecker.Data.Models.GGSheetModels;
using ntchecker.Extensions;
using ntchecker.Services.Interfaces;

namespace ntchecker.Services;

public class SalaryService : ISalaryService
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

    public SalaryService()
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
    private async Task<List<Data.Models.GGSheetModels.Salary>> Gets()
    {
        var dts = new List<Salary>();
        var range = $"{sheetSALARIES}!A2:Y";
        var values = await sheetsService.ltvGetSheetValuesAsync(SpreadSheetId, range);
        if (values == null || values.Count == 0)
        {
            throw new Exception("Không có dữ liệu sheet.");
        }

        foreach (var item in values)
        {
            dts.Add(new Salary
            {
                userId = item.ltvGetValue(0),
                revenue = item.ltvGetValue(1),
                tripsTotal = Convert.ToInt32(item.ltvGetValueDecimal(2)),
                kilometer = Convert.ToInt32(item.ltvGetValueDecimal(3)),
                kilometerWithCustomer = Convert.ToInt32(item.ltvGetValueDecimal(4)),
                businessDays = Convert.ToInt32(item.ltvGetValueDecimal(5)),
                salaryBase = item.ltvGetValue(6),
                deductForDeposit = item.ltvGetValue(7),//Trừ ký quỹ 
                deductForAccident = item.ltvGetValue(8),//Trừ tai nạn
                deductForSalaryAdvance = item.ltvGetValue(9),//Trừ lương ứng
                deductForViolationReport = item.ltvGetValue(10),//Trừ vi phạm biên bản
                deductForSocialInsurance = item.ltvGetValue(11),//Trừ BHXH
                deductForPIT = item.ltvGetValue(12),//Trừ TNCN - Personal Income Tax Deduction 
                deductForVMV = item.ltvGetValue(13),//Lỗi bảo quản xe: Vehicle Maintenance Violation
                deductForUV = item.ltvGetValue(14),//Lỗi đồng phục: Uniform Violation
                deductForSHV = item.ltvGetValue(15),//Lỗi giao ca: Shift Handover Violation
                deductForChargingPenalty = item.ltvGetValue(16),//Lỗi giao ca: Charging Penalty
                deductForTollPayment = item.ltvGetValue(17), //Trừ tiền qua trạm : Deduction for Toll Payment
                deductForOrderSalaryAdvance = item.ltvGetValue(18),//Trừ tạm ứng: nợ doanh thu, hoặc ứng tiền vì mục đích nào đó, kế toán cho phép
                deductForNegativeSalary = item.ltvGetValue(19),//Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương
                deductForOrder = item.ltvGetValue(20),//Trừ khác
                noteDeductOrder = item.ltvGetValue(21),//Ghi chú trừ khác
                deductTotal = item.ltvGetValue(22), //Tổng trừ
                salaryNet = item.ltvGetValue(23),//Lương thực nhận
                salaryDate = item.ltvGetValue(24),//Tháng/năm
            });
        }

        return dts;
    }

    // Lọc lại danh sách theo mã userId của tài xế [Họ tên - Mã nhân viên]
    // Gọi service Bank(_sheetBank) để add vào Revenue
    public async Task<Data.Models.GGSheetModels.Salary> GetSalary(string userId)
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
}
