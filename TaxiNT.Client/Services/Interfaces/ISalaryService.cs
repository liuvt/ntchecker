using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;   // Salary & SalaryDetails đã chuyển về namespace này

namespace TaxiNT.Client.Services.Interfaces;

public interface ISalaryService
{
    // 2. Lấy đầy đủ Salary + Details (1 API)
    Task<SalaryFullResponseDto> GetSalaryFull(string userId, string? date = null);

}
