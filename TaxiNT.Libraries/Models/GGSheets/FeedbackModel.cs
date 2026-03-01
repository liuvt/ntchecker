namespace TaxiNT.Libraries.Models.GGSheets;

public sealed class FeedbackModel
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [System.ComponentModel.DataAnnotations.StringLength(80)]
    public string? FullName { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [System.ComponentModel.DataAnnotations.Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? Phone { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn ngày xảy ra")]
    public DateTime? OccurredDate { get; set; } = DateTime.Today;

    [System.ComponentModel.DataAnnotations.StringLength(160)]
    public string? LocationOrRoute { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn loại phản ánh")]
    public string? Category { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(80)]
    public string? Reference { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập nội dung phản ánh")]
    [System.ComponentModel.DataAnnotations.StringLength(2000, MinimumLength = 10, ErrorMessage = "Nội dung tối thiểu 10 ký tự")]
    public string? Content { get; set; }
}

