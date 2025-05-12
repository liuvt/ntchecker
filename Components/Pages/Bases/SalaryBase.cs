using ntchecker.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using ntchecker.Extensions;

namespace ntchecker.Components.Pages.Bases;
public class SalaryBase : ComponentBase
{
    [Parameter]
    public string userId { get; set; }
    [Inject]
    protected ISalaryService salaryService { get; set; }
    protected Data.Models.GGSheetModels.Salary salary { get; set; } = new Data.Models.GGSheetModels.Salary();
    protected List<Data.Models.GGSheetModels.DeductionItem> Deductions { get; set; } = new List<Data.Models.GGSheetModels.DeductionItem>();
    protected string errorMessage { get; set; } = string.Empty;
    protected bool isLoaded = false;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            userId = Uri.UnescapeDataString(userId); // Chuyển chuỗi URL sang Tên - Mã Nhân Viên
            salary = await GetReSalary(userId);
            isLoaded = true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            Console.WriteLine($"Error: {errorMessage}");
            isLoaded = false;
        }
    }

    private async Task<Data.Models.GGSheetModels.Salary> GetReSalary(string userId)
    {
        var salary = await salaryService.GetSalary(userId);

        Deductions.AddRange(new List<Data.Models.GGSheetModels.DeductionItem>
        {
            new() { Name = "Ký quỹ", Amount = salary.deductForDeposit},
            new() { Name = "Tai nạn", Amount = salary.deductForAccident},
            new() { Name = "Lương ứng", Amount = salary.deductForSalaryAdvance },
            new() { Name = "Vi phạm biên bản", Amount = salary.deductForViolationReport },
            new() { Name = "BHXH", Amount = salary.deductForSocialInsurance },
            new() { Name = "TNCN", Amount = salary.deductForPIT },
            new() { Name = "Phạt bảo quản xe", Amount = salary.deductForVMV },
            new() { Name = "Phạt đồng phục", Amount = salary.deductForUV },
            new() { Name = "Phạt giao ca", Amount = salary.deductForSHV },
            new() { Name = "Phạt sạc", Amount = salary.deductForChargingPenalty },
            new() { Name = "Qua trạm", Amount = salary.deductForTollPayment },
            new() { Name = "Tạm ứng", Amount = salary.deductForOrderSalaryAdvance },
            new() { Name = "Âm lương", Amount = salary.deductForNegativeSalary },
            new() { Name = "Trừ khác", Amount = salary.deductForOrder }
        });

        return salary;
    }
}