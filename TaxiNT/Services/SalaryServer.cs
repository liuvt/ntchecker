
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
    public async Task<SalaryFullResponse?> GetSalaryFull(string userId, string? date = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId không được để trống");

        var query = _context.Salaries
            .AsNoTracking()
            .Where(s => s.userId.ToLower() == userId.ToLower());

        // Khai báo biến lưu trữ bản ghi lương tìm được
        Salary targetSalary = null;

        if (!string.IsNullOrWhiteSpace(date))
        {
            // TRƯỜNG HỢP 1: CÓ TRUYỀN DATE -> Phải tìm chính xác
            var trimmedDate = date.Trim();
            targetSalary = await query
                .Where(s => s.salaryDate == trimmedDate)
                .FirstOrDefaultAsync();

            // Nếu truyền sai date hoặc không có dữ liệu -> TRẢ VỀ NULL (Không trả kết quả)
            if (targetSalary == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu cho tháng này userId: {userId}");
            }
        }
        else
        {
            // TRƯỜNG HỢP 2: KHÔNG TRUYỀN DATE -> Lấy bản ghi mới nhất
            targetSalary = await query
                .OrderByDescending(s => s.salaryDate)
                .FirstOrDefaultAsync();

            // Nếu người dùng chưa từng có bảng lương nào
            if (targetSalary == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu lương cho userId: {userId}");
            }
        }

        var detailsList = await _context.SalaryDetails
            .AsNoTracking()
            .Where(d => d.salaryId == targetSalary.Id)
            .OrderBy(d => d.daterevenues)
            .ToListAsync();

        return new SalaryFullResponse
        {
            Salary = targetSalary,
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
    public async Task<List<SalaryUpsertResult>> UpsertSalary(List<SalaryUpsertRequest> requests)
    {
        var results = new List<SalaryUpsertResult>();

        if (requests == null || requests.Count == 0)
            return results;

        var now = DateTime.Now;

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Chuẩn hóa request hợp lệ
            var validRequests = requests
                .Where(r => r.Salary != null
                         && !string.IsNullOrWhiteSpace(r.Salary.userId)
                         && !string.IsNullOrWhiteSpace(r.Salary.salaryDate))
                .ToList();

            if (validRequests.Count == 0)
                return results;

            var userIds = validRequests
                .Select(r => r.Salary.userId.Trim())
                .Distinct()
                .ToList();

            var salaryDates = validRequests
                .Select(r => r.Salary.salaryDate!.Trim())
                .Distinct()
                .ToList();

            // Lấy các Salary hiện có theo userId + salaryDate
            var existingSalaries = await _context.Salaries
                .Where(s => userIds.Contains(s.userId) && salaryDates.Contains(s.salaryDate))
                .ToListAsync();

            foreach (var request in validRequests)
            {
                var salaryInput = request.Salary;
                var detailsInput = request.Details ?? new List<SalaryDetails>();
                var deductDetailsInput = request.DeductDetails ?? new List<SalaryDeductDetail>();

                var userId = salaryInput.userId.Trim();
                var salaryDate = salaryInput.salaryDate!.Trim();

                var existing = existingSalaries.FirstOrDefault(s =>
                    s.userId.Trim().ToLower() == userId.ToLower()
                    && (s.salaryDate ?? "").Trim() == salaryDate);

                Salary targetSalary;

                if (existing != null)
                {
                    // ===== Update Salary =====
                    existing.revenue = salaryInput.revenue;
                    existing.tripsTotal = salaryInput.tripsTotal;
                    existing.salaryType = salaryInput.salaryType;
                    existing.businessDays = salaryInput.businessDays;
                    existing.salaryBase = salaryInput.salaryBase;
                    existing.noteDeductOrder = salaryInput.noteDeductOrder;
                    existing.salaryDate = salaryDate;
                    existing.area = salaryInput.area;

                    targetSalary = existing;
                }
                else
                {
                    // ===== Insert Salary mới =====
                    salaryInput.Id = Guid.NewGuid().ToString();
                    salaryInput.userId = userId;
                    salaryInput.salaryDate = salaryDate;
                    salaryInput.createdAt = now;

                    await _context.Salaries.AddAsync(salaryInput);

                    targetSalary = salaryInput;
                }

                // ===== Xóa SalaryDetails cũ =====
                var oldDetails = await _context.SalaryDetails
                    .Where(d => d.salaryId == targetSalary.Id)
                    .ToListAsync();

                if (oldDetails.Any())
                {
                    _context.SalaryDetails.RemoveRange(oldDetails);
                }

                // ===== Xóa SalaryDeductDetails cũ =====
                var oldDeductDetails = await _context.SalaryDeductDetails
                    .Where(d => d.SalaryId == targetSalary.Id)
                    .ToListAsync();

                if (oldDeductDetails.Any())
                {
                    _context.SalaryDeductDetails.RemoveRange(oldDeductDetails);
                }

                // ===== Thêm SalaryDetails mới =====
                foreach (var detail in detailsInput)
                {
                    detail.Id = Guid.NewGuid().ToString();
                    detail.salaryId = targetSalary.Id;
                    detail.userId = targetSalary.userId;
                    detail.salaryDate = targetSalary.salaryDate;
                    detail.area = targetSalary.area;
                    detail.createdAt = now;
                }

                if (detailsInput.Any())
                {
                    await _context.SalaryDetails.AddRangeAsync(detailsInput);
                }

                // ===== Thêm SalaryDeductDetails mới =====
                foreach (var deduct in deductDetailsInput)
                {
                    deduct.Id = Guid.NewGuid().ToString();
                    deduct.SalaryId = targetSalary.Id;
                    deduct.CreatedAt = now;

                    // Không gán navigation để tránh EF hiểu nhầm insert lại Salary / DeductCategory
                    deduct.Salary = null;
                    deduct.DeductCategory = null;
                }

                if (deductDetailsInput.Any())
                {
                    await _context.SalaryDeductDetails.AddRangeAsync(deductDetailsInput);
                }

                // ===== Tính tổng trừ và thực nhận =====
                var deductTotal = deductDetailsInput.Sum(x => x.Amount);

                targetSalary.deductTotal = deductTotal;
                targetSalary.salaryNet = (targetSalary.salaryBase ?? 0) - deductTotal;

                results.Add(new SalaryUpsertResult
                {
                    Success = true,
                    SalaryId = targetSalary.Id,
                    Message = "Success",
                    DetailsCount = detailsInput.Count,
                    DeductDetailsCount = deductDetailsInput.Count
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return results;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return requests.Select(r => new SalaryUpsertResult
            {
                Success = false,
                Message = "Lỗi transaction",
                Errors = { ex.InnerException?.Message ?? ex.Message }
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
