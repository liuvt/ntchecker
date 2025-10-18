using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Services.Interfaces;
public interface ISalaryServer
{
    Task<Salary> GetSalary(string userId);
    Task<List<SalaryDetails>> GetSalaryDetails(string userId);
}
