namespace TaxiNT.Libraries.Models.GGSheets;
public class Salary
{
    public string userId { get; set; } = string.Empty; //Mã tài xế
    public string revenue { get; set; } = string.Empty;//Doanh thu
    public int tripsTotal { get; set; } //Số cuốc
    public string salaryType { get; set; } = string.Empty; // Loại hình kinh doanh LƯƠNG NGÀY - LƯƠNG THÁNG
    public int businessDays { get; set; } //Số ngày KD
    public string salaryBase { get; set; } = string.Empty; //Sau mức ăn chia || Lương cơ bản
    public string deductTotal { get; set; } = string.Empty; //Tổng trừ
    public string salaryNet { get; set; } = string.Empty; //Lương thực nhận

    public string deductForDeposit { get; set; } = string.Empty;//1. Trừ ký quỹ
    public string deductForSalaryAdvance { get; set; } = string.Empty;//2. Trừ lương ứng
    public string deductForNegativeSalary { get; set; } = string.Empty; //3. Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương (công ty)
    public string deductForViolationReport { get; set; } = string.Empty;//4. Trừ vi phạm biên bản  
    public string no_sua_chua { get; set; } = string.Empty; //5. Nợ sửa chữa
    public string haomon_voxe { get; set; } = string.Empty; //6. Hao mòn vỏ xe
    public string deductForCharging { get; set; } = string.Empty;//7. Sat pin
    public string deductForChargingPenalty { get; set; } = string.Empty; //8. phạt sạt: Charging Penalty
    public string deductForTollPayment { get; set; } = string.Empty;//9. Trừ tiền qua trạm : Deduction for Toll Payment
    public string deductForSocialInsurance { get; set; } = string.Empty;//10. Trừ BHXH
    public string deductForNegativeSalaryPartner { get; set; } = string.Empty; //11. Trừ âm lương: Nợ tiền tháng trước, qua tháng này trừ lại vào lương (Thương quyền)
    public string deductForPIT { get; set; } = string.Empty;//12. Trừ TNCN - Personal Income Tax Deduction 
    public string deductForOrder { get; set; } = string.Empty; //13. Trừ khác
    public string noteDeductOrder { get; set; } = string.Empty; //14. Ghi chú trừ khác

    public string salaryDate { get; set; } = string.Empty; //Tháng/năm
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
