using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Models.GGSheets;   // Dùng cho các model GGSheet khác (GGSUser, Feedback...)

namespace TaxiNT.Services.Interfaces;
public interface ISalaryServer
{
    // Lấy đầy đủ Salary + SalaryDetails trong 1 lần gọi (dùng cho hiển thị)
    Task<SalaryFullResponse> GetSalaryFull(string userId, string? date = null);

    // 3. Upsert toàn bộ (Salary + Details) - Hỗ trợ 1 list (dùng cho bulk từ Google Apps Script)
    // Mỗi record được xử lý trong transaction riêng, rollback riêng nếu lỗi
    Task<List<SalaryFullUpsertResult>> UpsertFullSalary(List<SalaryFullUpsertRequest> requests);

    // Feedback (giữ lại nếu vẫn dùng)
    Task AddAsync(FeedbackModel model);
}
