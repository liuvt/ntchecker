
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using TaxiNT.Data;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Models.GGSheets;   // Giữ cho GGSUser, FeedbackModel
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
    private readonly string sheetFeelback = "Feedback";

    // SQL
    private readonly taxiNTDBContext _context;

    public SalaryServer(taxiNTDBContext context)
    {
        this._context = context;

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
    #endregion Constructor

    /// <summary>
    /// Lấy đầy đủ Salary + SalaryDetails trong một lần gọi (dùng cho Client hiển thị).
    /// </summary>
    public async Task<SalaryFullResponse> GetSalaryFull(string userId, string? date = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId không được để trống");

        var query = _context.Salaries
            .AsNoTracking()
            .Where(s => s.userId.ToLower() == userId.ToLower());

        if (!string.IsNullOrWhiteSpace(date))
        {
            var trimmedDate = date.Trim();
            var specific = await query
                .Where(s => s.salaryDate == trimmedDate)
                .FirstOrDefaultAsync();

            if (specific != null)
            {
                var details = await _context.SalaryDetails
                    .AsNoTracking()
                    .Where(d => d.salaryId == specific.Id)
                    .OrderBy(d => d.daterevenues)
                    .ToListAsync();

                return new SalaryFullResponse
                {
                    Salary = specific,
                    Details = details
                };
            }
        }

        // Lấy bản ghi mới nhất
        var latestSalary = await query
            .OrderByDescending(s => s.salaryDate)
            .FirstOrDefaultAsync();

        if (latestSalary == null)
        {
            throw new Exception($"Không tìm thấy dữ liệu lương cho userId: {userId}");
        }

        var detailsList = await _context.SalaryDetails
            .AsNoTracking()
            .Where(d => d.salaryId == latestSalary.Id)
            .OrderBy(d => d.daterevenues)
            .ToListAsync();

        return new SalaryFullResponse
        {
            Salary = latestSalary,
            Details = detailsList
        };
    }

    /// <summary>
    /// Upsert 1 list Salary + SalaryDetails trong 1 request duy nhất.
    /// Mô hình xử lý giống UpsertShiftWorkDailyAsync:
    /// - 1 transaction lớn cho toàn bộ batch
    /// - Xóa các SalaryDetails cũ không còn trong batch
    /// - Upsert Salary + thay thế toàn bộ Details
    /// </summary>
    public async Task<List<SalaryFullUpsertResult>> UpsertFullSalary(List<SalaryFullUpsertRequest> requests)
    {
        var results = new List<SalaryFullUpsertResult>();

        if (requests == null || requests.Count == 0)
            return results;

        var now = DateTime.Now;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // === Bước 1: Thu thập key từ request ===
            var incomingSalaryKeys = requests
                .Where(r => r.Salary != null)
                .Select(r => new
                {
                    r.Salary.userId,
                    r.Salary.salaryDate,
                    Request = r
                })
                .ToList();

            var userIds = incomingSalaryKeys.Select(x => x.userId).Distinct().ToList();
            var salaryDates = incomingSalaryKeys.Select(x => x.salaryDate).Distinct().ToList();

            // === Bước 2: Lấy các Salary hiện có ===
            var existingSalaries = await _context.Salaries
                .Where(s => userIds.Contains(s.userId) && salaryDates.Contains(s.salaryDate))
                .ToListAsync();

            // === Bước 3: Xóa SalaryDetails cũ không còn trong batch (theo key) ===
            // Lấy tất cả detail keys đang được gửi
            var allIncomingDetailKeys = incomingSalaryKeys
                .SelectMany(x => x.Request.Details ?? new List<SalaryDetails>())
                .Select(d => $"{(d.userId ?? "").Trim().ToLower()}|{(d.daterevenues ?? "").Trim()}")
                .Distinct()
                .ToList();

            // Xóa các details thuộc các Salary trong batch nhưng không nằm trong incoming keys
            var salariesToClean = existingSalaries.Select(s => s.Id).ToList();

            if (salariesToClean.Any())
            {
                var obsoleteDetails = await _context.SalaryDetails
                    .Where(d => salariesToClean.Contains(d.salaryId) &&
                               !allIncomingDetailKeys.Contains(
                                   $"{(d.userId ?? "").Trim().ToLower()}|{(d.daterevenues ?? "").Trim()}"))
                    .ToListAsync();

                if (obsoleteDetails.Any())
                {
                    _context.SalaryDetails.RemoveRange(obsoleteDetails);
                    await _context.SaveChangesAsync();
                }
            }

            // === Bước 4: Xử lý từng Salary ===
            foreach (var item in incomingSalaryKeys)
            {
                var salaryInput = item.Request.Salary;
                var detailsInput = item.Request.Details ?? new List<SalaryDetails>();

                var existing = existingSalaries
                    .FirstOrDefault(s => s.userId == salaryInput.userId && s.salaryDate == salaryInput.salaryDate);

                Salary targetSalary;

                if (existing != null)
                {
                    // Update Salary
                    existing.revenue = salaryInput.revenue;
                    existing.tripsTotal = salaryInput.tripsTotal;
                    existing.salaryType = salaryInput.salaryType;
                    existing.businessDays = salaryInput.businessDays;
                    existing.salaryBase = salaryInput.salaryBase;
                    existing.deductTotal = salaryInput.deductTotal;
                    existing.salaryNet = salaryInput.salaryNet;

                    // Cập nhật các khoản trừ
                    existing.deductForDeposit = salaryInput.deductForDeposit;
                    existing.deductForSalaryAdvance = salaryInput.deductForSalaryAdvance;
                    existing.deductForNegativeSalary = salaryInput.deductForNegativeSalary;
                    existing.deductForViolationReport = salaryInput.deductForViolationReport;
                    existing.no_sua_chua = salaryInput.no_sua_chua;
                    existing.haomon_voxe = salaryInput.haomon_voxe;
                    existing.deductForCharging = salaryInput.deductForCharging;
                    existing.deductForChargingPenalty = salaryInput.deductForChargingPenalty;
                    existing.deductForTollPayment = salaryInput.deductForTollPayment;
                    existing.deductForSocialInsurance = salaryInput.deductForSocialInsurance;
                    existing.deductForNegativeSalaryPartner = salaryInput.deductForNegativeSalaryPartner;
                    existing.deductForPIT = salaryInput.deductForPIT;
                    existing.deductForOrder = salaryInput.deductForOrder;
                    existing.noteDeductOrder = salaryInput.noteDeductOrder;

                    existing.updatedAt = now;

                    targetSalary = existing;
                }
                else
                {
                    // Insert Salary mới
                    salaryInput.Id = Guid.NewGuid().ToString();
                    salaryInput.createdAt = now;
                    salaryInput.updatedAt = null;

                    await _context.Salaries.AddAsync(salaryInput);
                    targetSalary = salaryInput;
                }

                await _context.SaveChangesAsync();

                // Xóa toàn bộ details cũ của Salary này (đã xóa một phần ở trên, xóa nốt cho chắc)
                var oldDetails = _context.SalaryDetails.Where(d => d.salaryId == targetSalary.Id);
                _context.SalaryDetails.RemoveRange(oldDetails);

                // Gán salaryId và thêm details mới
                foreach (var d in detailsInput)
                {
                    d.Id = Guid.NewGuid().ToString();
                    d.salaryId = targetSalary.Id;
                    d.userId = targetSalary.userId;
                    d.createdAt = now;
                    d.updatedAt = null;
                }

                await _context.SalaryDetails.AddRangeAsync(detailsInput);

                results.Add(new SalaryFullUpsertResult
                {
                    Success = true,
                    SalaryId = targetSalary.Id,
                    Message = "Success",
                    DetailsCount = detailsInput.Count
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return results;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Trả về lỗi cho toàn bộ batch
            return requests.Select(r => new SalaryFullUpsertResult
            {
                Success = false,
                Message = "Lỗi transaction",
                Errors = { ex.Message }
            }).ToList();
        }
    }

    #region Feedback
    public async Task AddAsync(FeedbackModel model)
    {
        var values = new List<object?>
    {
        Guid.NewGuid().ToString(),
        model.FullName?.Trim(),
        model.Phone?.Trim(),
        model.OccurredDate?.ToString("yyyy-MM-dd"), // ISO để không lỗi locale
        model.LocationOrRoute?.Trim(),
        model.Category?.Trim(),
        model.Reference?.Trim(),
        model.Content?.Trim(),
        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
    };

        var valueRange = new ValueRange
        {
            Values = new List<IList<object?>> { values }
        };

        await sheetsService.ltvAppendSheetValuesAsync(
            SpreadSheetId,
            $"{sheetFeelback}!A:I", //Nếu dòng 1 có dữ liệu sẽ không bị ghi đè vì đã sử dụng ltvAppendSheetValuesAsync, nếu dòng 1 không có dữ liệu sẽ ghi vào đó
            valueRange);
    }
    #endregion Feedback
}
