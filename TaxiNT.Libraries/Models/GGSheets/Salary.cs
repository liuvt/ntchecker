namespace TaxiNT.Libraries.Models.GGSheets;
public class Salary
{
    public string userId { get; set; } = string.Empty; //Mã tài xế
    public string revenue { get; set; } = string.Empty;//Doanh thu
    public int tripsTotal { get; set; } //Số cuốc
    //public int kilometer { get; set; } //Số km vận doanh
    //public int kilometerWithCustomer { get; set; } //Số km có khách
    public int businessDays { get; set; } //Số ngày KD
    public string salaryBase { get; set; } = string.Empty; //Sau mức ăn chia || Lương cơ bản
    public string deductForDeposit { get; set; } = string.Empty;//Trừ ký quỹ 
    public string deductForAccident { get; set; } = string.Empty;//Trừ tai nạn
    public string deductForSalaryAdvance { get; set; } = string.Empty;//Trừ lương ứng
    public string deductForViolationReport { get; set; } = string.Empty;//Trừ vi phạm biên bản  
    public string deductForSocialInsurance { get; set; } = string.Empty;//Trừ BHXH
    public string deductForPIT { get; set; } = string.Empty;//Trừ BHXH//Trừ TNCN - Personal Income Tax Deduction 
    public string deductForVMV { get; set; } = string.Empty;//Lỗi bảo quản xe: Vehicle Maintenance Violation
    public string deductForUV { get; set; } = string.Empty;//Lỗi đồng phục: Uniform Violation
    public string deductForSHV { get; set; } = string.Empty; //Lỗi giao ca: Shift Handover Violation
    public string deductForChargingPenalty { get; set; } = string.Empty; //phạt sạt: Charging Penalty
    public string deductForCharging { get; set; } = string.Empty;//Sat pin
    public string deductForTollPayment { get; set; } = string.Empty;//Trừ tiền qua trạm : Deduction for Toll Payment
    public string deductForOrder { get; set; } = string.Empty; //Trừ khác
    public string noteDeductOrder { get; set; } = string.Empty; //Ghi chú trừ khác
    public string deductForOrderSalaryAdvance { get; set; } = string.Empty; //Trừ tạm ứng: nợ doanh thu, hoặc ứng tiền vì mục đích nào đó, kế toán cho phép
    public string deductForNegativeSalary { get; set; } = string.Empty; //Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương
    public string no_sua_chua { get; set; } = string.Empty; //Nợ sửa chữa
    public string deductTotal { get; set; } = string.Empty; //Tổng trừ
    public string salaryNet { get; set; } = string.Empty; //Lương thực nhận
    public string salaryDate { get; set; } = string.Empty; //Tháng/năm
    public string salaryType { get; set; } = string.Empty; //Tháng/năm
}


public class DeductionItem
{
    public string Name { get; set; } = string.Empty;
    public string NameAlias { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
}

public class SalaryDetails
{
    public string userId { get; set; } = string.Empty; // Mã tài xế
    public string revenue { get; set; } = string.Empty;// Doanh thu
    public string revenueAC { get; set; } = string.Empty;// Doanh thu AC giảm 5% GTGT
    public string type { get; set; } = string.Empty; // Loai hinh kinh doanh LƯƠNG NGÀY - LƯƠNG THÁNG
    public string salaryBase { get; set; } = string.Empty; // Sau mức ăn chia
    public string daterevenues { get; set; } = string.Empty;
    public string createdAt { get; set; } = string.Empty;
}
