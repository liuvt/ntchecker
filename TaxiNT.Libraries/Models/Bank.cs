using System.ComponentModel.DataAnnotations;

namespace TaxiNT.Libraries.Models;
public class ModelBank
{
    [Key]
    public string bank_Id { get; set; } = string.Empty;
    public string bank_NumberId { get; set; } = string.Empty;
    public string bank_Name { get; set; } = string.Empty;
    public string bank_NumberCard { get; set; } = string.Empty;
    public string bank_Type { get; set; } = string.Empty;
    public string bank_AccountName { get; set; } = string.Empty;
    public string bank_Url { get; set; } = string.Empty;
    public bool bank_Status { get; set; }
    public DateTime createdAt { get; set; } 
    public DateTime? updatedAt { get; set; }
}

public class UpsertBanksResult
{
    public List<ModelBank> InsertResults { get; set; } = new();
    public List<ModelBank> UpdateResults { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public int InsertedCounts => InsertResults.Count;
    public int UpdatedCounts => UpdateResults.Count;
    public int ErrorCounts => Errors.Count;
}

public class DeleteBanksResult
{
    public List<string> Results { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public int ResultCounts => Results.Count;        
    public int ErrorCounts => Errors.Count;
}
