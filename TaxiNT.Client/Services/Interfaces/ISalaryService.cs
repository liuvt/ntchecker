using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Client.Services.Interfaces;

public interface ISalaryService
{
    Task<Salary> GetSalary(string userId);
    Task<List<SalaryDetails>> GetSalaryDetails(string userId);
}
