namespace ntchecker.Services.Interfaces;
public interface ISalaryService
{
    Task<Data.Models.GGSheetModels.Salary> GetSalary(string userId);
}
