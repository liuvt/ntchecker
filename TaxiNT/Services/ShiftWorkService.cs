using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiNT.Data;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Extensions;
using TaxiNT.Libraries.MapperModels;
using TaxiNT.Libraries.Models;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Services;
//WithSQL
public class ShiftWorkService : IShiftWorkService
{
    #region SQL Controctor
    private readonly taxiNTDBContext _context;
    private readonly ILogger<ShiftWorkService> _logger;

    public ShiftWorkService(taxiNTDBContext context, ILogger<ShiftWorkService> logger)
    {
        this._context = context;
        this._logger = logger;
    }
    #endregion

    #region CURD
    // Upsert dữ liệu hằng ngày cho 3 bảng
    public async Task<object> UpsertShiftWorkDailyAsync(ShiftWorkDailySyncDto data)
    {
        // kiểm tra dữ liệu đầu vào
        if (data?.ShiftWorks == null || data.ShiftWorks.Count == 0)
            return new BadRequestObjectResult("No ShiftWork data found.");

        //Tạo transaction để đảm bảo tính toàn vẹn dữ liệu. Tất cả sẽ Rollback nếu trong quá trình create/update/delete có lỗi xảy ra
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Đếm tổng số bản ghi
            int totalTrips = 0;
            int totalContracts = 0;
            int totalDelete = 0;

            #region Xóa các ShiftWork: Trips/Contracts củ không có trong batch hiện tại
            // === Khởi tạo key vì không xác định ID: User - Area - Date ===
            var incomingKeys = data.ShiftWorks
                .Where(g => g.ShiftWork != null)
                .Select(g => new
                {
                    g.ShiftWork.numberCar,
                    g.ShiftWork.userId,
                    g.ShiftWork.area,
                    WorkDate = g.ShiftWork.createdAt?.Date
                })
                .ToList();

            // Xóa dữ liệu củ không có trong batch hiện tại theo key:  Area - Date
            // var allUserIds = incomingKeys.Select(k => k.userId).Distinct().ToList(); 
            var allAreas = incomingKeys.Select(k => k.area).Distinct().ToList();
            var allDates = incomingKeys.Select(k => k.WorkDate).Distinct().ToList();

            // === Lấy Toàn bộ dữ liệu shiftwork SQL theo key: Area - Date ===
            var existingShiftworks = await _context.ShiftWorks
                .Where(sw => allAreas.Contains(sw.area)
                          && sw.createdAt.HasValue //Kiểm tra trước xem có null không
                          && allDates.Contains(sw.createdAt.Value.Date))
                .Include(sw => sw.Trips)
                .Include(sw => sw.Contracts)
                .ToListAsync();

            // === Tìm ShiftWork cũ không có trong batch mới ===
            var obsoleteShiftworks = existingShiftworks
                .Where(old => !incomingKeys.Any(k =>
                    k.numberCar == old.numberCar && // Tìm trong batch mới có số xe trùng không
                    k.userId == old.userId &&
                    k.area == old.area &&
                    k.WorkDate == old.createdAt.Value.Date))
                .ToList();

            // Xóa các ShiftWork cũ cùng với Trips và Contracts liên quan
            if (obsoleteShiftworks.Any())
            {
                var obsoleteIds = obsoleteShiftworks.Select(sw => sw.Id).ToList();

                // Log chi tiết những bản ghi bị xóa
                foreach (var sw in obsoleteShiftworks)
                {
                    _logger.LogWarning(
                        "🗑 Xóa ShiftWork: User = {UserId}, Ngày = {WorkDate}, Khu vực = {Area}, ShiftWorkId = {Id}",
                        sw.numberCar,
                        sw.userId,
                        sw.createdAt?.ToString("yyyy-MM-dd"),
                        sw.area,
                        sw.Id
                    );
                }

                _context.Trips.RemoveRange(_context.Trips.Where(t => obsoleteIds.Contains(t.shiftworkId)));
                _context.Contracts.RemoveRange(_context.Contracts.Where(c => obsoleteIds.Contains(c.shiftworkId)));
                _context.ShiftWorks.RemoveRange(obsoleteShiftworks);

                totalDelete = obsoleteShiftworks.Count;
                await _context.SaveChangesAsync();
            }
            #endregion

            // Xử lý từng nhóm ShiftWork
            foreach (var group in data.ShiftWorks)
            {
                // Lấy thông tin chung của 1 tài xế thông qua shiftWork: Khu vực + Tài xế + Ngày
                #region Xữ lý dữ liệu ShiftWork
                var sw = group.ShiftWork;
                if (sw.createdAt == null)
                    throw new Exception("ShiftWork.createdAt is required to determine WorkDate.");

                var workDate = sw.createdAt.Value.Date;
                var area = sw.area;
                var userId = sw.userId;
                var numberCar = sw.numberCar;

                // Tìm ShiftWork hiện có theo Area + NumberCar + User + Ngày 
                var existingShift = await _context.ShiftWorks
                    .FirstOrDefaultAsync(x =>
                        x.area == area &&
                        x.numberCar == numberCar &&
                        x.userId == userId &&
                        x.createdAt.HasValue && //Kiểm tra trước khi null
                        x.createdAt.Value.Date == workDate);

                // Khởi tạo biến để giữ ShiftWork mục tiêu (mới hoặc cập nhật)
                ShiftWork targetShift;

                //Kiểm tra tồn tại thì cập nhật, không thì thêm mới
                if (existingShift != null)
                {
                    // --- Update từng property để EF nhận thay đổi ---
                    existingShift.numberCar = sw.numberCar;
                    existingShift.userId = sw.userId;
                    existingShift.revenueByMonth = sw.revenueByMonth;
                    existingShift.revenueByDate = sw.revenueByDate;
                    existingShift.qrContext = sw.qrContext;
                    existingShift.qrUrl = sw.qrUrl;
                    existingShift.discountOther = sw.discountOther;
                    existingShift.arrearsOther = sw.arrearsOther;
                    existingShift.totalPrice = sw.totalPrice;
                    existingShift.walletGSM = sw.walletGSM;
                    existingShift.discountGSM = sw.discountGSM;
                    existingShift.discountNT = sw.discountNT;
                    existingShift.bank_Id = sw.bank_Id;
                    existingShift.createdAt = sw.createdAt;
                    existingShift.typeCar = sw.typeCar;
                    existingShift.area = sw.area;
                    existingShift.ranking = sw.ranking;
                    existingShift.basicSalary = sw.basicSalary;

                    // Log cập nhật ShiftWork
                    _logger.LogInformation(
                        "🔁 Cập nhật ShiftWork: User = {UserId}, Ngày = {WorkDate}, Khu vực = {Area}, Id = {Id}",
                        existingShift.userId,
                        existingShift.numberCar,
                        existingShift.createdAt?.ToString("yyyy-MM-dd"),
                        existingShift.area,
                        existingShift.Id
                    );

                    //Ghi lại dữ liệu để cập nhật
                    targetShift = existingShift;
                }
                else
                {
                    // --- Thêm mới ShiftWork ---
                    await _context.ShiftWorks.AddAsync(sw);

                    // Log thêm mới ShiftWork
                    _logger.LogInformation(
                        "🆕 Thêm mới ShiftWork: User = {UserId}, Ngày = {WorkDate}, Khu vực = {Area}",
                        sw.userId,
                        sw.createdAt?.ToString("yyyy-MM-dd"),
                        sw.area
                    );

                    //Ghi lại liệu để thêm mới
                    targetShift = sw;
                }

                //Ghi vào SQL
                await _context.SaveChangesAsync();
                #endregion

                #region Xữ lý dữ liệu Trip và Contract theo khóa ngoại shiftworkId Xóa và cập nhật mới
                //Lấy Id của ShiftWork vừa thêm hoặc cập nhật để xữ lý Trip và Contract
                var shiftworkId = targetShift.Id;

                // --- Xóa dữ liệu cũ ---
                //Tìm contract và trip theo ID của shiftwork
                var oldTrips = _context.Trips.Where(t => t.shiftworkId == shiftworkId);
                var oldContracts = _context.Contracts.Where(c => c.shiftworkId == shiftworkId);
                //Xoá dữ liệu cũ
                _context.Trips.RemoveRange(oldTrips);
                _context.Contracts.RemoveRange(oldContracts);
                //Ghi vào SQL
                await _context.SaveChangesAsync();

                // --- Gán shiftworkId cho dữ liệu Trips mới ---
                foreach (var trip in group.Trips)
                {
                    trip.shiftworkId = shiftworkId;
                }
                // --- Gán shiftworkId cho dữ liệu Contracts mới ---
                foreach (var contract in group.Contracts)
                {
                    contract.shiftworkId = shiftworkId;
                }

                // --- Thêm dữ liệu mới ---
                await _context.Trips.AddRangeAsync(group.Trips);
                await _context.Contracts.AddRangeAsync(group.Contracts);

                //Ghi vào SQL lần cuối
                await _context.SaveChangesAsync();
                #endregion

                // Cập nhật tổng số bản ghi
                totalTrips += group.Trips.Count;
                totalContracts += group.Contracts.Count;
            }

            // Commit transaction nếu tất cả thành công sẽ được ghi vào database
            await transaction.CommitAsync();

            return new OkObjectResult(new
            {
                message = "Upsert completed successfully",
                totalShiftWorks = data.ShiftWorks.Count,
                totalTrips,
                totalContracts,
                totalDelete
            });
        }
        catch (Exception ex)
        {
            // Rollback transaction nếu có lỗi xảy ra, toàn bộ thay đổi sẽ bị hủy không tác động vào database
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error during UpsertShiftWorkDailyAsync");
            return new ObjectResult("Internal server error") { StatusCode = 500 };
        }
        /* Example input data upsert:
     {
      "shiftWorks": [
            {
              "shiftWork": {
                "numberCar": "BL3001",
                "userId": "LÊ HOÀNG HẾT - BL0109",
                "revenueByMonth": 2248000,
                "revenueByDate": 1232500,
                "qrContext": "BL3001 - LÊ HOÀNG HẾT - BL0109 - 07102025",
                "qrUrl": "https://img.vietqr.io/image/",
                "discountOther": 0,
                "arrearsOther": 0,
                "totalPrice": 1232500,
                "walletGSM": 0,
                "discountGSM": 0,
                "discountNT": 0,
                "bank_Id": "BLBANK0001",
                "createdAt": "2025-10-07T00:00:00.000Z",
                "typeCar": "Taxi điện",
                "Area": "BACLIEU",
                "Rank": 51,
                "SauMucAnChia": 542300
              },
              "trips": [
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T10:36:43.000Z",
                  "tpTimeEnd": "2025-10-07T10:42:23.000Z",
                  "tpDistance": 1.55,
                  "tpPrice": 27000,
                  "tpPickUp": "Phường Láng Tròn, Cà Mau",
                  "tpDropOut": "Phường Láng Tròn, Cà Mau",
                  "tpType": "Cuốc Lẻ",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T13:27:03.000Z",
                  "tpTimeEnd": "2025-10-07T13:32:19.000Z",
                  "tpDistance": 2.07,
                  "tpPrice": 36000,
                  "tpPickUp": "Đường QL1A, Phường Giá Rai, Cà Mau",
                  "tpDropOut": "Đường Gía Cần Bảy, Phường Giá Rai, Cà Mau",
                  "tpType": "Xanh SM",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T15:53:41.000Z",
                  "tpTimeEnd": "2025-10-07T16:06:24.000Z",
                  "tpDistance": 8.23,
                  "tpPrice": 107000,
                  "tpPickUp": "Đường QL1A, Phường Giá Rai, Cà Mau",
                  "tpDropOut": "Đường Cầu Hộ Phòng, Phường Giá Rai, Cà Mau",
                  "tpType": "Cuốc Lẻ",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T17:24:12.000Z",
                  "tpTimeEnd": "2025-10-07T17:33:46.000Z",
                  "tpDistance": 4.7,
                  "tpPrice": 64500,
                  "tpPickUp": "Đường QL1A, Phường Giá Rai, Cà Mau",
                  "tpDropOut": "Đường QL1A, Phường Giá Rai, Cà Mau",
                  "tpType": "Cuốc Lẻ",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T18:22:03.000Z",
                  "tpTimeEnd": "2025-10-07T18:29:50.000Z",
                  "tpDistance": 6.68,
                  "tpPrice": 88500,
                  "tpPickUp": "Đường QL1A, Xã Phong Thạnh, Cà Mau",
                  "tpDropOut": "Đường Trần Văn Sớm, Phường Giá Rai, Cà Mau",
                  "tpType": "Cuốc Lẻ",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T19:02:33.000Z",
                  "tpTimeEnd": "2025-10-07T19:06:24.000Z",
                  "tpDistance": 1.53,
                  "tpPrice": 26500,
                  "tpPickUp": "Đường Trần Văn Sớm, Phường Giá Rai, Cà Mau",
                  "tpDropOut": "Đường QL1A, Phường Giá Rai, Cà Mau",
                  "tpType": "Cuốc Lẻ",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "tpTimeStart": "2025-10-07T19:20:55.000Z",
                  "tpTimeEnd": "2025-10-07T19:27:36.000Z",
                  "tpDistance": 5.23,
                  "tpPrice": 71000,
                  "tpPickUp": "Đường QL1A, Phường Giá Rai, Cà Mau",
                  "tpDropOut": "Đường QL1A, Xã Phong Thạnh, Cà Mau",
                  "tpType": "Cuốc Lẻ",
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                }
              ],
              "contracts": [
                {
                  "numberCar": "BL3001",
                  "ctKey": "Hộ Phòng -> CÀ MAU",
                  "ctAmout": 396000,
                  "ctDefaultDistance": "62km - 60 phút",
                  "ctOverDistance": "9km - 73 phút",
                  "ctSurcharge": 127000,
                  "ctPromotion": 0,
                  "totalPrice": 523000,
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                },
                {
                  "numberCar": "BL3001",
                  "ctKey": "Hộ Phòng -> Gành hào",
                  "ctAmout": 255000,
                  "ctDefaultDistance": "30km - 60 phút",
                  "ctOverDistance": "4km - 0 phút",
                  "ctSurcharge": 34000,
                  "ctPromotion": 0,
                  "totalPrice": 289000,
                  "userId": "LÊ HOÀNG HẾT - BL0109",
                  "createdAt": "2025-10-07T00:00:00.000Z"
                }
              ]
            }
        ]
       }
     
     */
    }

    ///Gets data
    public async Task<ShiftWorkDto> Gets(string userId, string date)
    {
        // kiểm tra dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(date))
            throw new ArgumentException("userId and date are required.");
        if (!DateTime.TryParse(date, out DateTime workDate))
            throw new ArgumentException("Invalid date format. Expected format: yyyy-MM-dd");

        /// Lấy toàn bộ ShiftWork theo User và Date
        var result = await _context.ShiftWorks
            .Where(sw => sw.userId == userId
                      && sw.createdAt.HasValue
                      && sw.createdAt.Value.Date == workDate.Date)
            .Include(sw => sw.Trips)
            .Include(sw => sw.Contracts)
            .ToListAsync();

        // Chuyển đổi dữ liệu sang DTO
        var dataMap = result.ToDto();

        /// Lấy số tài khoản ngân hàng từ SQL để tạo QR
        var banks = await _context.Banks.Where(b => b.bank_Id == dataMap.bank_Id).FirstOrDefaultAsync();

        if (banks != null)
        {
            // Tạo chuỗi QR
            var qrString = $"{dataMap.numberCar} {dataMap.userId.Replace("-", "").Replace(" ", "")} {workDate:ddMMyyyy}";
            dataMap.qrContext = qrString;
            
            // Tạo URL QR code sử dụng dịch vụ VietQR
            var qrUrl = $"https://img.vietqr.io/image/{banks.bank_NumberId}-{banks.bank_NumberCard}-{banks.bank_Type}?amount={dataMap.totalPrice.ltvVNDCurrency().Replace(".","")}&addInfo={Uri.EscapeDataString(qrString)}&accountName={Uri.EscapeDataString(banks.bank_AccountName)}";
            dataMap.qrUrl = qrUrl;
        }
        else
        {
            dataMap.qrContext = "Bank information not found.";
            dataMap.qrUrl = string.Empty;
        }
        return dataMap;
    } 

    #endregion
}
