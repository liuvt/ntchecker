using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Services.Interfaces;
public interface IShiftWorkService
{
    /// Cập nhật hoặc tạo mới ca làm việc hàng ngày, nếu ID tồn
    Task<object> UpsertShiftWorkDailyAsync(ShiftWorkDailySyncDto data);
    /// Lấy ca làm việc theo userId và ngày (định dạng "yyyy-MM-dd"), nếu date null thì lấy ngày hiện tại -1 để thu tiền phiếu ngày hôm qua
    Task<ShiftWorkDto?> Gets(string userId, string date);
    /// Cập nhật hoặc tạo mới ca làm việc theo ID, nếu ID tồn tại thì cập nhật, nếu không tồn tại thì tạo mới
    Task<object> UpsertShiftWorkDailyByIdAsync(ShiftWorkUpsertByIdDto data);
    /// Lấy danh sách ShiftWorkDto theo area và createdAt (ngày tạo)
    Task<List<ShiftWorkDto>> GetShiftWorkDtosByAreaAndCreatedAtAsync(string area, string createdAt);
}
