using System.ComponentModel.DataAnnotations;

namespace ntchecker.Data.Models;

public class QuickLink
{
    [Key]
    public string ql_Id { get; set; } = string.Empty; //GuilID
    public string ql_Url { get; set; } = string.Empty; // https://img.vietqr.io/image/<BANK_ID>-<ACCOUNT_NO>-<TEMPLATE>.png?amount=<AMOUNT>&addInfo=<DESCRIPTION>&accountName=<ACCOUNT_NAME>
    public string ql_BankId { get; set; } = string.Empty; // Mã BIN ngân hàng, được quy định bởi ngân hàng nhà nước || danh sách GET: https://api.vietqr.io/v2/banks
    public string ql_AccountNo { get; set; } = string.Empty; // Số tài khoản
    public string ql_template { get; set; } = string.Empty; // Hiển thị QR | gọi TemplateType
    public double? ql_amount { get; set; } // Số tiền
    public string ql_description { get; set; } = string.Empty; // Nội dung chuyển khoản
    public string ql_AccountName { get; set; } = string.Empty; // Tên người thụ hưởng
    public bool Activity { get; set; } = true; // Trạng thái hoạt động
    public DateTime CreatedAt { get; set; } = DateTime.Now; // Trạng thái hoạt động
}

public static class TemplateType
{
    public const string Compact2 = "compact2";
    public const string Compact = "compact";
    public const string QrOnly = "qr_only";
    public const string Print = "print";
}

//API lấy bankId : https://api.vietqr.io/v2/banks
public class BankBinV2 
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty; // Tên pháp nhân ngân hàng
    public string code { get; set; } = string.Empty; // Tên mã ngân hàng
    public int bin { get; set; } //Mã ngân hàng, sử dụng mã này trong Quick Link
    public string shortName { get; set; } = string.Empty; //Tên ngắn gọn thường gọi
    public string logo { get; set; } = string.Empty; //	Đường dẫn tới logo ngân hàng
    public int transferSupported { get; set; } //App của Bank hỗ trợ chuyển tiền bằng cách quét mã VietQR
    public int lookupSupported { get; set; } // Số tài khoản của Bank có thể tra cứu bằng API tra cứu số tài khoản
    public string short_name { get; set; } = string.Empty; //Tương tự shortName (sẽ bỏ field này ở v3)
    public int support { get; set; } //Mức độ hỗ trợ VietQR của ngân hàng (sẽ bỏ field này ở v3)
    public int isTransfer { get; set; } // Tương tự transferSupported (sẽ bỏ field này ở v3)
    public string swift_code { get; set; } = string.Empty; //SWIFT Code (hay còn được gọi là BIC Code) là mã định danh duy nhất của một tổ chức tài chính hoặc ngân hàng trên thế giới
}