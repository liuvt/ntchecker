using System.Collections.Generic;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.Entities;

/// <summary>
/// Request DTO cho API Full Upsert Salary + SalaryDetails (chi tiết)
/// </summary>
public class SalaryFullUpsertRequest
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

/// <summary>
/// Response DTO cho kết quả Full Upsert Salary + Details
/// </summary>
public class SalaryFullUpsertResult
{
    public bool Success { get; set; }
    public string? SalaryId { get; set; }
    public string Message { get; set; } = string.Empty;
    public int DetailsCount { get; set; }
    public List<string> Errors { get; set; } = new();
}