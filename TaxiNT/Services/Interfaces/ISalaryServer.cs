using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;   // Dùng cho các model GGSheet khác (GGSUser, Feedback...)

namespace TaxiNT.Services.Interfaces;
public interface ISalaryServer
{
    #region Test - Ver old code, keep if still used
    // Lấy đầy đủ Salary + SalaryDetails trong 1 lần gọi (dùng cho hiển thị)
    Task<SalaryFullResponseDto> GetSalaryFull(string userId, string? date = null);
    #endregion

    // Feedback cho guest phản ánh ở Index Page (giữ lại nếu vẫn dùng)
    Task AddAsync(FeedbackModel model);

    /// Main
    #region Main
    // Lấy đầy đủ Salary + DeductDetail + SalaryDetails trong 1 lần gọi (dùng cho hiển thị)
    Task<SalaryResponseDto> GetSalaryByUserId(string userId, string? date = null);

    // Upsert toàn bộ (Salary + Deduct + Details). Mỗi record được xử lý trong transaction riêng, rollback riêng nếu lỗi
    Task<List<SalaryUpsertResult>> UpsertSalary(List<SalaryUpsertRequest> requests);


    //Delete
    Task<List<SalaryDeleteResult>> SalaryDeleteList(List<SalaryDeleteRequest> requests);
    Task<int> SalaryDeleteByAreaAndDate(SalaryDeleteByAreaRequest req);
    #endregion

}
