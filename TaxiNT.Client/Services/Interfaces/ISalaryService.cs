using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;   // Salary & SalaryDetails đã chuyển về namespace này

namespace TaxiNT.Client.Services.Interfaces;

public interface ISalaryService
{
    // Lấy đầy đủ Salary + Details (1 API)
    Task<SalaryFullResponseDto> GetSalaryFull(string userId, string? date = null);

    // Lấy đầy đủ Salary + Details + Deduct (1 API)
    Task<SalaryResponseDto> GetSalaryByUser(string userId, string? date = null);

}
