using System.ComponentModel.DataAnnotations;

namespace TaxiNT.Libraries.Entities;
public class UserDto
{
}
public class LoginUserDto : UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserDto
{
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
    //Mật khẩu yêu cầu: 8-15 ký tự, 1 ký tự đặt biệt (!,#,$,%,..), 1 ký tự viết hoa, 1 chữ số. Ví dụ: Abc!1234
    [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\\]*+\\/|!\"£$%^&*()#[@~'?><,.=_-]).{5,15}$"
        , ErrorMessage = "Yêu cầu: 4-15 ký tự, 1 ký tự đặt biệt (!,#,$,%,..), 1 ký tự viết hoa, 1 chữ số. Ví dụ: Abc!1234.")]
    public string Password { get; set; } = string.Empty;


    [Compare(nameof(Password), ErrorMessage = "Nhập lại mật khẩu không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

//Update
public class EditUserDto
{
    [Required(ErrorMessage = "Họ không được bỏ trống.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên không được bỏ trống.")]
    public string LastName { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
    [RegularExpression(@"((84|60|86|02|01|0)[1-9]{1})+(([0-9]{8})|([0-9]{9})|([0-9]{10}))",
                                                    ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ không được bỏ trống.")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Giới tính không được bỏ trống.")]
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDay { get; set; }
}


//Change password
public class ChangePasswordUserDto
{
    [Required(ErrorMessage = "Không được bỏ trống.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được bỏ trống.")]
    //Mật khẩu yêu cầu: 8-15 ký tự, 1 ký tự đặt biệt (!,#,$,%,..), 1 ký tự viết hoa, 1 chữ số. Ví dụ: Abc!1234
    [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[\\]*+\\/|!\"£$%^&*()#[@~'?><,.=_-]).{8,15}$"
        , ErrorMessage = "Yêu cầu: 8-15 ký tự, 1 ký tự đặt biệt (!,#,$,%,..), 1 ký tự viết hoa, 1 chữ số. Ví dụ: Abc!1234.")]
    public string Password { get; set; } = string.Empty;

    [Compare(nameof(Password), ErrorMessage = "Nhập lại mật khẩu không khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}