using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiNT.Data;
using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Extensions;
using TaxiNT.Libraries.MapperModels;
using TaxiNT.Libraries.Models;
using TaxiNT.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    }

    ///Upsert dữ liệu theo ShiftWorkId, nếu có Id thì cập nhật, không có Id thì thêm mới
    public async Task<object> UpsertShiftWorkDailyByIdAsync(ShiftWorkUpsertByIdDto data)
    {
        // kiểm tra dữ liệu đầu vào
        if (data?.ShiftWorks == null || data.ShiftWorks.Count == 0)
            return new BadRequestObjectResult("No ShiftWork data found.");

        // Chỉ check duplicate với các Id có giá trị
        var duplicateShiftIds = data.ShiftWorks
            .Select(x => x.ShiftWork.Id)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateShiftIds.Any())
            return new BadRequestObjectResult(
                $"Duplicate ShiftWork.Id found: {string.Join(", ", duplicateShiftIds)}");

        // Tạo transaction để đảm bảo tính toàn vẹn dữ liệu. Tất cả sẽ Rollback nếu trong quá trình create/update/delete có lỗi xảy ra
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            int totalTrips = 0;
            int totalContracts = 0;

            int totalInsertShiftWorks = 0;
            int totalUpdateShiftWorks = 0;

            int totalDeleteTrips = 0;
            int totalInsertTrips = 0;

            int totalDeleteContracts = 0;
            int totalInsertContracts = 0;

            // Lấy tất cả ShiftWorkId từ dữ liệu đầu vào để truy vấn dữ liệu hiện có trong database, giúp tối ưu số lần truy vấn
            var incomingShiftIds = data.ShiftWorks
                .Select(x => x.ShiftWork.Id)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            // Tạo một dictionary để map ShiftWorkId với entity hiện có, giúp truy cập nhanh khi cần cập nhật
            var existingShiftMap = await _context.ShiftWorks
                .Where(x => incomingShiftIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id);

            // Xử lý từng nhóm ShiftWork
            foreach (var group in data.ShiftWorks)
            {
                var swDto = group.ShiftWork;
                var incomingTripDtos = swDto.Trips ?? new List<TripDto>();
                var incomingContractDtos = swDto.Contracts ?? new List<ContractDto>();

                // Biến để giữ ShiftWork mục tiêu (có thể là mới hoặc đã cập nhật)
                ShiftWork targetShift;

                #region Upsert ShiftWork theo Id
                // Kiểm tra nếu có Id và tồn tại trong database thì cập nhật, ngược lại thì thêm mới
                if (!string.IsNullOrWhiteSpace(swDto.Id) &&
                    existingShiftMap.TryGetValue(swDto.Id, out var existingShift))
                {

                    targetShift = swDto.ToEntity(existingShift); //MapperModels để cập nhật dữ liệu vào entity cũ
                    totalUpdateShiftWorks++;

                    _logger.LogInformation(
                        "🔁 Update ShiftWork: ShiftWorkId={ShiftWorkId}, UserId={UserId}, NumberCar={NumberCar}, Date={Date}, Area={Area}",
                        existingShift.Id,
                        existingShift.userId,
                        existingShift.numberCar,
                        existingShift.createdAt?.ToString("yyyy-MM-dd"),
                        existingShift.area
                    );
                }
                else
                {
                    var newShift = swDto.ToEntity();

                    if (string.IsNullOrWhiteSpace(newShift.Id))
                        newShift.Id = Guid.NewGuid().ToString();

                    await _context.ShiftWorks.AddAsync(newShift);

                    targetShift = newShift;
                    totalInsertShiftWorks++;

                    _logger.LogInformation(
                        "🆕 Insert ShiftWork: ShiftWorkId={ShiftWorkId}, UserId={UserId}, NumberCar={NumberCar}, Date={Date}, Area={Area}",
                        newShift.Id,
                        newShift.userId,
                        newShift.numberCar,
                        newShift.createdAt?.ToString("yyyy-MM-dd"),
                        newShift.area
                    );
                }
                #endregion

                // Ghi vào SQL để có Id của ShiftWork mới (nếu là mới) hoặc đảm bảo entity đã được attach vào context (nếu là cập nhật)
                var shiftworkId = targetShift.Id;

                #region Xóa Trips cũ theo shiftworkId
                // Tìm Trips cũ theo ID của shiftwork
                var oldTrips = await _context.Trips
                    .Where(t => t.shiftworkId == shiftworkId)
                    .ToListAsync();
                // Xóa dữ liệu cũ
                if (oldTrips.Any())
                {
                    _context.Trips.RemoveRange(oldTrips);
                    totalDeleteTrips += oldTrips.Count;

                    _logger.LogInformation(
                        "🗑 Delete all Trips by ShiftWorkId={ShiftWorkId}, Count={Count}",
                        shiftworkId,
                        oldTrips.Count
                    );
                }
                #endregion

                #region Xóa Contracts cũ theo shiftworkId
                // Tìm Contracts cũ theo ID của shiftwork
                var oldContracts = await _context.Contracts
                    .Where(c => c.shiftworkId == shiftworkId)
                    .ToListAsync();
                // Xóa dữ liệu cũ
                if (oldContracts.Any())
                {
                    _context.Contracts.RemoveRange(oldContracts);
                    totalDeleteContracts += oldContracts.Count;

                    _logger.LogInformation(
                        "🗑 Delete all Contracts by ShiftWorkId={ShiftWorkId}, Count={Count}",
                        shiftworkId,
                        oldContracts.Count
                    );
                }
                #endregion

                #region Insert Trips mới
                // Gán shiftworkId cho dữ liệu Trips mới
                var newTrips = incomingTripDtos
                    .Select(x => x.TripDtoToEntity(shiftworkId))
                    .ToList();
                // Thêm dữ liệu mới
                if (newTrips.Any())
                {
                    await _context.Trips.AddRangeAsync(newTrips);
                    totalInsertTrips += newTrips.Count;

                    _logger.LogInformation(
                        "➕ Insert Trips by ShiftWorkId={ShiftWorkId}, Count={Count}",
                        shiftworkId,
                        newTrips.Count
                    );
                }
                #endregion

                #region Insert Contracts mới
                // Gán shiftworkId cho dữ liệu Contracts mới
                var newContracts = incomingContractDtos
                    .Select(x => x.ContractDtoToEntity(shiftworkId))
                    .ToList();
                // Thêm dữ liệu mới
                if (newContracts.Any())
                {
                    await _context.Contracts.AddRangeAsync(newContracts);
                    totalInsertContracts += newContracts.Count;

                    _logger.LogInformation(
                        "➕ Insert Contracts by ShiftWorkId={ShiftWorkId}, Count={Count}",
                        shiftworkId,
                        newContracts.Count
                    );
                }
                #endregion

                totalTrips += newTrips.Count;
                totalContracts += newContracts.Count;
            }

            await _context.SaveChangesAsync();
            // Commit transaction nếu tất cả thành công sẽ được ghi vào database
            await transaction.CommitAsync();

            return new
            {
                message = "Upsert by ShiftWorkId completed successfully",
                totalShiftWorks = data.ShiftWorks.Count,
                totalTrips,
                totalContracts,

                totalInsertShiftWorks,
                totalUpdateShiftWorks,

                totalDeleteTrips,
                totalInsertTrips,

                totalDeleteContracts,
                totalInsertContracts
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error during UpsertShiftWorkDailyByIdAsync");

            return new ObjectResult("Internal server error")
            {
                StatusCode = 500
            };
        }
    }

    ///Gets data by Area + Date
    public async Task<List<ShiftWorkDto>> GetShiftWorkDtosByAreaAndCreatedAtAsync(string area, string createdAt)
    {
        if (string.IsNullOrWhiteSpace(area))
            return new List<ShiftWorkDto>();

        if (!DateTime.TryParse(createdAt, out DateTime _workDate))
            throw new ArgumentException("Invalid date format. Expected format: yyyy-MM-dd");

        var workDate = _workDate.Date;

        var data = await _context.ShiftWorks
            .AsNoTracking()
            .Where(sw =>
                sw.area == area &&
                sw.createdAt.HasValue &&
                sw.createdAt.Value.Date == workDate)
            .Include(sw => sw.Trips)
            .Include(sw => sw.Contracts)
            .OrderBy(sw => sw.numberCar)
            .ToListAsync();

        return data.Select(sw => new ShiftWorkDto
        {
            Id = sw.Id,
            numberCar = sw.numberCar,
            userId = sw.userId,
            revenueByMonth = sw.revenueByMonth,
            revenueByDate = sw.revenueByDate,
            qrContext = sw.qrContext,
            qrUrl = sw.qrUrl,
            discountOther = sw.discountOther,
            arrearsOther = sw.arrearsOther,
            totalPrice = sw.totalPrice,
            walletGSM = sw.walletGSM,
            discountGSM = sw.discountGSM,
            discountNT = sw.discountNT,
            bank_Id = sw.bank_Id,
            createdAt = sw.createdAt,
            typeCar = sw.typeCar,
            area = sw.area,
            ranking = sw.ranking,
            basicSalary = sw.basicSalary,

            Trips = (sw.Trips ?? new List<Trip>())
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    NumberCar = t.NumberCar,
                    tpTimeStart = t.tpTimeStart,
                    tpTimeEnd = t.tpTimeEnd,
                    tpDistance = t.tpDistance,
                    tpPrice = t.tpPrice,
                    tpPickUp = t.tpPickUp,
                    tpDropOut = t.tpDropOut,
                    tpType = t.tpType,
                    userId = t.userId,
                    createdAt = t.createdAt
                })
                .ToList(),

            Contracts = (sw.Contracts ?? new List<Contract>())
                .Select(c => new ContractDto
                {
                    ctId = c.ctId,
                    numberCar = c.numberCar,
                    ctKey = c.ctKey,
                    ctAmount = c.ctAmount,
                    ctDefaultDistance = c.ctDefaultDistance,
                    ctOverDistance = c.ctOverDistance,
                    ctSurcharge = c.ctSurcharge,
                    ctPromotion = c.ctPromotion,
                    totalPrice = c.totalPrice,
                    userId = c.userId,
                    createdAt = c.createdAt
                })
                .ToList()
        }).ToList();
    }

    ///Gets data by User + Date để hiển thị chi tiết ca làm việc của từng tài xế
    public async Task<ShiftWorkDto?> Gets(string userId, string date)
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
            var qrUrl = $"https://img.vietqr.io/image/{banks.bank_NumberId}-{banks.bank_NumberCard}-{banks.bank_Type}?amount={dataMap.totalPrice.ltvVNDCurrency().Replace(".", "")}&addInfo={Uri.EscapeDataString(qrString)}&accountName={Uri.EscapeDataString(banks.bank_AccountName)}";
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