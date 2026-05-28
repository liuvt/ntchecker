using System.Collections.Generic;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.Entities;

/// <summary>
/// Response DTO cho API GetFullSalary - trả về đầy đủ Salary + SalaryDetails
/// </summary>
public class SalaryFullResponse
{
    /// <summary>
    /// Thông tin tổng hợp lương (bản ghi chính)
    /// </summary>
    public Salary Salary { get; set; } = new();

    /// <summary>
    /// Danh sách chi tiết lương theo ngày (daily breakdown)
    /// </summary>
    public List<SalaryDetails> Details { get; set; } = new();
}