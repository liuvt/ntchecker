using System.ComponentModel.DataAnnotations;

namespace ntchecker.Data.Entities;

public class QuickLinkDto
{
    public string ql_Id { get; set; } = string.Empty; //GuilID
    public string ql_Url { get; set; } = string.Empty; // https://img.vietqr.io/image/<BANK_ID>-<ACCOUNT_NO>-<TEMPLATE>.png?amount=<AMOUNT>&addInfo=<DESCRIPTION>&accountName=<ACCOUNT_NAME>
    public string ql_BankId { get; set; } = string.Empty; // Mã BIN ngân hàng, được quy định bởi ngân hàng nhà nước || danh sách GET: https://api.vietqr.io/v2/banks
    public string ql_AccountNo { get; set; } = string.Empty; // Số tài khoản
    public string ql_template { get; set; } = string.Empty; // Hiển thị QR | gọi TemplateType
    public double? ql_amount { get; set; } // Số tiền
    public string ql_description { get; set; } = string.Empty; // Nội dung chuyển khoản
    public string ql_AccountName { get; set; } = string.Empty; // Tên người thụ hưởng
}
