
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.EntityFrameworkCore;
using TaxiNT.Data;
using TaxiNT.Extensions;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.MapperModels;
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

    //For Sheet
    private readonly string sheetFeelback = "Feedback";

    //SQL
    private readonly taxiNTDBContext _context;

    public SalaryServer(taxiNTDBContext context)
    {
        this._context = context;

        //  File xác thực google tài khoản
        GoogleCredential credential;
        using (var stream = new FileStream(CredentialGGSheetService, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(Scopes);
        }

        //Đăng ký service
        sheetsService = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = AppName,
        });
    }
    #endregion Constructor

    /// <summary>
    // Lấy đầy đủ Salary + SalaryDetails trong một lần gọi(dùng cho Client hiển thị).
    ///</summary>
    public async Task<SalaryFullResponseDto?> GetSalaryFull(string userId, string? date = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId không được để trống");

        var query = _context.Salaries
            .AsNoTracking()
            .Where(s => s.userId.ToLower() == userId.ToLower());

        //Khai báo biến lưu trữ bản ghi lương tìm được
        Salary targetSalary = null;

        if (!string.IsNullOrWhiteSpace(date))
        {
            //TRƯỜNG HỢP 1: CÓ TRUYỀN DATE->Phải tìm chính xác
            var trimmedDate = date.Trim();
            targetSalary = await query
                .Where(s => s.salaryDate == trimmedDate)
                .FirstOrDefaultAsync();

            // Nếu truyền sai date hoặc không có dữ liệu->TRẢ VỀ NULL(Không trả kết quả)
            if (targetSalary == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu cho tháng này userId: {userId}");
            }
        }
        else
        {
            //TRƯỜNG HỢP 2: KHÔNG TRUYỀN DATE->Lấy bản ghi mới nhất
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

        return new SalaryFullResponseDto
        {
            Salary = targetSalary,
            Details = detailsList
        };
    }


    #region Main

    /// <summary>
    // Lấy đầy đủ Salary + SalaryDetails + SalaryDeductDetails trong một lần gọi.
    // Sử dụng Mapper để map sang SalaryResponseDto có cấu trúc phù hợp với Client.
    // Dùng cho Client hiển thị bảng lương chi tiết.
    /// </summary>
    public async Task<SalaryResponseDto?> GetSalaryByUserId(string userId, string? date = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId không được để trống");

        var normalizedUserId = userId.Trim().ToLower();

        var query = _context.Salaries
            .AsNoTracking()
            .Where(s => s.userId.Trim().ToLower() == normalizedUserId);

        Salary? targetSalary;

        if (!string.IsNullOrWhiteSpace(date))
        {
            var trimmedDate = date.Trim();

            targetSalary = await query
                .Where(s => s.salaryDate == trimmedDate)
                .FirstOrDefaultAsync();

            if (targetSalary == null)
            {
                throw new Exception($"Không tìm thấy dữ liệu lương cho userId: {userId}, tháng: {trimmedDate}");
            }
        }
        else
        {
            targetSalary = await query
                .OrderByDescending(s => s.createdAt)
                .FirstOrDefaultAsync();

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

        var deductDetailsList = await _context.SalaryDeductDetails
            .AsNoTracking()
            .Where(d => d.SalaryId == targetSalary.Id)
            .OrderBy(d => d.DeductCategory != null ? d.DeductCategory.SortOrder : 9999)
            .ThenBy(d => d.CreatedAt)
            .Select(d => new SalaryDeductDetailResponse
            {
                Id = d.Id,
                SalaryId = d.SalaryId,
                DeductCategoryId = d.DeductCategoryId,
                Code = d.DeductCategory != null ? d.DeductCategory.Code : "",
                Name = d.DeductCategory != null ? d.DeductCategory.Name : "",
                Amount = d.Amount,
                Note = d.Note,
                CreatedAt = d.CreatedAt
            })
            .ToListAsync();

        return SalaryMapperGetByUser.ToSalaryResponseDto(
            targetSalary,
            detailsList,
            deductDetailsList
        );
    }

    /// <summary>
    /// Upsert 1 list Salary + SalaryDetails + SalaryDeductDetails trong 1 request.
    /// Lưu ý: Hàm này dùng 1 transaction chung cho toàn bộ batch.
    /// Nếu SaveChanges lỗi, toàn bộ batch sẽ rollback.
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
            var validRequests = GetValidSalaryRequests(requests);

            if (validRequests.Count == 0)
                return results;

            var existingSalaries = await GetExistingSalaries(validRequests);

            var deductCategories = await GetDeductCategories(validRequests);

            foreach (var request in validRequests)
            {
                var salaryInput = request.Salary;
                var detailsInput = request.Details ?? new List<SalaryDetailUpsertDto>();
                var deductDetailsInput = request.DeductDetails ?? new List<SalaryDeductDetailUpsertDto>();

                var userId = salaryInput.userId.Trim();
                var salaryDate = salaryInput.salaryDate!.Trim();

                var recordErrors = ValidateDeductCodes(deductDetailsInput, deductCategories);

                if (recordErrors.Any())
                {
                    results.Add(new SalaryUpsertResult
                    {
                        Success = false,
                        SalaryId = "",
                        Message = "Lỗi dữ liệu khoản trừ",
                        DetailsCount = detailsInput.Count,
                        DeductDetailsCount = deductDetailsInput.Count,
                        Errors = recordErrors
                    });

                    continue;
                }

                var targetSalary = await CreateOrUpdateSalary(
                    existingSalaries,
                    salaryInput,
                    userId,
                    salaryDate,
                    now
                );

                await RemoveOldSalaryDetails(targetSalary.Id);

                await RemoveOldSalaryDeductDetails(targetSalary.Id);

                var newDetails = SalaryMapperUpsert.MapSalaryDetails(
                    detailsInput,
                    targetSalary,
                    now
                );

                if (newDetails.Any())
                {
                    await _context.SalaryDetails.AddRangeAsync(newDetails);
                }

                var newDeductDetails = SalaryMapperUpsert.MapSalaryDeductDetails(
                    deductDetailsInput,
                    deductCategories,
                    targetSalary,
                    now
                );

                if (newDeductDetails.Any())
                {
                    await _context.SalaryDeductDetails.AddRangeAsync(newDeductDetails);
                }

                var deductTotal = SalaryMapperUpsert.CalculateDeductTotal(newDeductDetails);

                SalaryMapperUpsert.ApplySalaryNet(targetSalary, deductTotal);

                results.Add(new SalaryUpsertResult
                {
                    Success = true,
                    SalaryId = targetSalary.Id,
                    Message = "Success",
                    DetailsCount = newDetails.Count,
                    DeductDetailsCount = newDeductDetails.Count
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

    #region Upsert Helper
    // Hàm này sẽ lấy ra những bản ghi Salary đã tồn tại trong database có userId + salaryDate tương ứng với những request hợp lệ trong batch.
    // Việc lấy ra existingSalaries này sẽ giúp cho hàm upsert biết được là nên tạo mới hay update khi xử lý từng request.
    private async Task<List<Salary>> GetExistingSalaries(List<SalaryUpsertRequest> validRequests)
    {
        var userIds = validRequests
            .Select(r => r.Salary.userId.Trim())
            .Distinct()
            .ToList();

        var salaryDates = validRequests
            .Select(r => r.Salary.salaryDate!.Trim())
            .Distinct()
            .ToList();

        return await _context.Salaries
            .Where(s => userIds.Contains(s.userId) && salaryDates.Contains(s.salaryDate))
            .ToListAsync();
    }

    // Lấy tất cả DeductCategory có code khớp với code trong request (dùng để validate và map khi upsert SalaryDeductDetail)
    private async Task<List<DeductCategory>> GetDeductCategories(List<SalaryUpsertRequest> validRequests)
    {
        var deductCodes = validRequests
            .SelectMany(r => r.DeductDetails ?? new List<SalaryDeductDetailUpsertDto>())
            .Select(x => x.Code?.Trim() ?? "")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return await _context.DeductCategories
            .Where(x => deductCodes.Contains(x.Code))
            .ToListAsync();
    }

    // Validate xem code khoản trừ trong request có tồn tại trong DeductCategory hay không. Nếu không tồn tại, sẽ trả về lỗi để client fix lại trước khi upsert.
    private static List<string> ValidateDeductCodes(
    List<SalaryDeductDetailUpsertDto> deductDetailsInput,
    List<DeductCategory> deductCategories)
    {
        var errors = new List<string>();

        foreach (var deduct in deductDetailsInput)
        {
            var code = deduct.Code?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(code))
            {
                errors.Add("Code khoản trừ không được để trống");
                continue;
            }

            var categoryExists = deductCategories.Any(x =>
                x.Code.Trim().Equals(code, StringComparison.OrdinalIgnoreCase));

            if (!categoryExists)
            {
                errors.Add($"Không tìm thấy DeductCategory với Code: {code}");
            }
        }

        return errors;
    }

    // Hàm này sẽ kiểm tra xem đã tồn tại bản ghi Salary nào với userId + salaryDate tương ứng hay chưa. Nếu đã tồn tại thì update, nếu chưa tồn tại thì tạo mới.
    private async Task<Salary> CreateOrUpdateSalary(
    List<Salary> existingSalaries,
    SalaryUpsertDto salaryInput,
    string userId,
    string salaryDate,
    DateTime now)
    {
        var existing = existingSalaries.FirstOrDefault(s =>
            s.userId.Trim().Equals(userId, StringComparison.OrdinalIgnoreCase)
            && (s.salaryDate ?? "").Trim() == salaryDate);

        if (existing != null)
        {
            SalaryMapperUpsert.MapUpdateSalary(
                existing,
                salaryInput,
                userId,
                salaryDate
            );

            return existing;
        }

        var newSalary = SalaryMapperUpsert.MapCreateSalary(
            salaryInput,
            userId,
            salaryDate,
            now
        );

        await _context.Salaries.AddAsync(newSalary);

        return newSalary;
    }

    // Khi upsert Salary, nếu có SalaryDetails cũ thì xóa hết đi để tránh tình trạng dư thừa hoặc dữ liệu không đồng nhất. Sau đó sẽ thêm lại toàn bộ SalaryDetails mới từ request vào.
    private async Task RemoveOldSalaryDetails(string salaryId)
    {
        var oldDetails = await _context.SalaryDetails
            .Where(d => d.salaryId == salaryId)
            .ToListAsync();

        if (oldDetails.Any())
        {
            _context.SalaryDetails.RemoveRange(oldDetails);
        }
    }

    // Tương tự như SalaryDetails, khi upsert Salary nếu có SalaryDeductDetails cũ thì xóa hết đi để tránh dư thừa hoặc dữ liệu không đồng nhất. Sau đó sẽ thêm lại toàn bộ SalaryDeductDetails mới từ request vào.
    private async Task RemoveOldSalaryDeductDetails(string salaryId)
    {
        var oldDeductDetails = await _context.SalaryDeductDetails
            .Where(d => d.SalaryId == salaryId)
            .ToListAsync();

        if (oldDeductDetails.Any())
        {
            _context.SalaryDeductDetails.RemoveRange(oldDeductDetails);
        }
    }

    // Hàm này sẽ lọc ra những request có thông tin Salary hợp lệ (có Salary không null, có userId và salaryDate) để tránh lỗi khi upsert vào database. Những request không hợp lệ sẽ bị bỏ qua và không được upsert.
    private static List<SalaryUpsertRequest> GetValidSalaryRequests(List<SalaryUpsertRequest> requests)
    {
        return requests
            .Where(r => r.Salary != null
                     && !string.IsNullOrWhiteSpace(r.Salary.userId)
                     && !string.IsNullOrWhiteSpace(r.Salary.salaryDate))
            .ToList();
    }
    #endregion

    #region Delete
    /// <summary>
    /// Xóa full Salary theo userId + salaryDate.
    /// Thứ tự xóa:
    /// 1. SalaryDeductDetails
    /// 2. SalaryDetails
    /// 3. Salaries
    /// </summary>
    private async Task<SalaryDeleteResult> SalaryDeleteByUserId(string userId, string salaryDate)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId không được để trống");

        if (string.IsNullOrWhiteSpace(salaryDate))
            throw new ArgumentException("salaryDate không được để trống");

        var normalizedUserId = userId.Trim();
        var normalizedSalaryDate = salaryDate.Trim();

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var salary = await _context.Salaries
                .FirstOrDefaultAsync(s =>
                    s.userId.Trim().ToLower() == normalizedUserId.ToLower()
                    && (s.salaryDate ?? "").Trim() == normalizedSalaryDate);

            if (salary == null)
            {
                return new SalaryDeleteResult
                {
                    Success = false,
                    userId = normalizedUserId,
                    salaryDate = normalizedSalaryDate,
                    Message = "Không tìm thấy dữ liệu lương để xóa"
                };
            }

            var salaryId = salary.Id;

            var deductDetails = await _context.SalaryDeductDetails
                .Where(x => x.SalaryId == salaryId)
                .ToListAsync();

            if (deductDetails.Any())
            {
                _context.SalaryDeductDetails.RemoveRange(deductDetails);
            }

            var details = await _context.SalaryDetails
                .Where(x => x.salaryId == salaryId)
                .ToListAsync();

            if (details.Any())
            {
                _context.SalaryDetails.RemoveRange(details);
            }

            _context.Salaries.Remove(salary);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SalaryDeleteResult
            {
                Success = true,
                SalaryId = salaryId,
                userId = normalizedUserId,
                salaryDate = normalizedSalaryDate,
                Message = "Xóa dữ liệu lương thành công"
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return new SalaryDeleteResult
            {
                Success = false,
                userId = normalizedUserId,
                salaryDate = normalizedSalaryDate,
                Message = "Lỗi khi xóa dữ liệu lương",
                Errors = { ex.InnerException?.Message ?? ex.Message }
            };
        }
    }

    /// <summary>
    /// Xóa full nhiều Salary trong 1 lần gọi.
    /// Mỗi record xử lý transaction riêng, record lỗi không ảnh hưởng record khác.
    /// </summary>
    /*
         [
          {
            "userId": "TX001",
            "salaryDate": "05/2026"
          },
          {
            "userId": "TX002",
            "salaryDate": "05/2026"
          }
        ]
     */
    public async Task<List<SalaryDeleteResult>> SalaryDeleteList(List<SalaryDeleteRequest> requests)
    {
        var results = new List<SalaryDeleteResult>();

        if (requests == null || requests.Count == 0)
            return results;

        var validRequests = requests
            .Where(x => !string.IsNullOrWhiteSpace(x.userId)
                     && !string.IsNullOrWhiteSpace(x.salaryDate))
            .ToList();

        if (validRequests.Count == 0)
            return results;

        foreach (var request in validRequests)
        {
            var result = await SalaryDeleteByUserId(
                request.userId,
                request.salaryDate
            );

            results.Add(result);
        }

        return results;
    }

    /* Xóa lương cả khu vực
        {
          "area": "Rạch Giá",
          "salaryDate": "05/2026"
        }
     */
    public async Task<int> SalaryDeleteByAreaAndDate(SalaryDeleteByAreaRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.area))
            throw new ArgumentException("area không được để trống");

        if (string.IsNullOrWhiteSpace(req.salaryDate))
            throw new ArgumentException("salaryDate không được để trống");

        var normalizedArea = req.area.Trim();
        var normalizedSalaryDate = req.salaryDate.Trim();

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var salaries = await _context.Salaries
                .Where(s =>
                    (s.area ?? "").Trim() == normalizedArea
                    && (s.salaryDate ?? "").Trim() == normalizedSalaryDate)
                .ToListAsync();

            if (!salaries.Any())
                return 0;

            var salaryIds = salaries.Select(x => x.Id).ToList();

            var deductDetails = await _context.SalaryDeductDetails
                .Where(x => salaryIds.Contains(x.SalaryId))
                .ToListAsync();

            if (deductDetails.Any())
            {
                _context.SalaryDeductDetails.RemoveRange(deductDetails);
            }

            var details = await _context.SalaryDetails
                .Where(x => salaryIds.Contains(x.salaryId))
                .ToListAsync();

            if (details.Any())
            {
                _context.SalaryDetails.RemoveRange(details);
            }

            _context.Salaries.RemoveRange(salaries);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return salaries.Count;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    #endregion


    #endregion

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
