using System.ComponentModel.DataAnnotations;

namespace TaxiNT.Libraries.Models.Zalos
{
    public class CustomerOrder
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required(ErrorMessage = "Tên không được bỏ trống.")]
        public string CO_Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống.")]
        [RegularExpression(@"((84|60|86|02|01|0)[1-9]{1})+(([0-9]{8})|([0-9]{9})|([0-9]{10}))",
                                                    ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string CO_Phone { get; set; } = string.Empty;
        public string CO_Locations { get; set; } = string.Empty;
        public string CO_TypeCar { get; set; } = string.Empty;
        public DateTime? CO_DateOrder { get; set; }
    }
}
