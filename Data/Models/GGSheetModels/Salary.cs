
namespace ntchecker.Data.Models.GGSheetModels;

public class Salary
{
    public string userId { get; set; } //Mã tài xế
    public decimal revenue { get; set; } //Doanh thu
    public int tripsTotal { get; set; } //Số cuốc
    public int kilometer { get; set; } //Số km vận doanh
    public int kilometerWithCustomer { get; set; } //Số km có khách
    public int businessDays { get; set; } //Số ngày KD
    public decimal salaryBase { get; set; } //Sau mức ăn chia || Lương cơ bản
    public decimal deductForDeposit { get; set; } //Trừ ký quỹ 
    public decimal deductForAccident { get; set; } //Trừ tai nạn
    public decimal deductForSalaryAdvance { get; set; } //Trừ lương ứng
    public decimal deductForViolationReport { get; set; } //Trừ vi phạm biên bản  
    public decimal deductForSocialInsurance { get; set; } //Trừ BHXH
    public decimal deductForPIT { get; set; } //Trừ BHXH//Trừ TNCN - Personal Income Tax Deduction 
    public decimal deductForVMV { get; set; } //Lỗi bảo quản xe: Vehicle Maintenance Violation
    public decimal deductForUV { get; set; } //Lỗi đồng phục: Uniform Violation
    public decimal deductForSHV { get; set; } //Lỗi giao ca: Shift Handover Violation
    public decimal deductForChargingPenalty { get; set; } //Lỗi giao ca: Charging Penalty
    public decimal deductForTollPayment { get; set; } //Trừ tiền qua trạm : Deduction for Toll Payment
    public decimal deductForOrderSalaryAdvance { get; set; } //Trừ tạm ứng: nợ doanh thu, hoặc ứng tiền vì mục đích nào đó, kế toán cho phép
    public decimal deductForNegativeSalary { get; set; } //Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương
    public decimal deductForOrder { get; set; } //Trừ khác
    public string noteDeductOrder { get; set; } //Ghi chú trừ khác
    public decimal deductTotal { get; set; } //Tổng trừ
    public decimal salaryNet { get; set; } //Lương thực nhận
    public string salaryDate { get; set; } //Tháng/năm
}

public class DeductionItem
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
}