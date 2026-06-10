using System.Collections.Generic;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.Entities;

/// <summary> 
public class SalaryUpsertRequest
{
    /// <summary>
    /// Thông tin tổng hợp lương (bản ghi chính)
    /// </summary>
    public Salary Salary { get; set; } = new();
    /// <summary>
    /// Danh sách chi tiết Khoản trừ lương (deduction breakdown)
    /// </summary>
    public List<SalaryDeductDetail>? DeductDetails { get; set; } = new();
    /// <summary>
    /// Danh sách chi tiết lương theo ngày (daily breakdown)
    /// </summary>
    public List<SalaryDetails>? Details { get; set; } = new();
}
public class SalaryUpsertResult
{
    public bool Success { get; set; }

    public string? SalaryId { get; set; }

    public string Message { get; set; } = string.Empty;

    public int DetailsCount { get; set; }

    public int DeductDetailsCount { get; set; }

    public List<string> Errors { get; set; } = new();
}