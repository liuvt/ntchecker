using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.MapperModels;

public static class ShiftWorkMapper
{
    // Hiển thị cho phiếu checker, 1 láy xe có thể chạy nhiều xe trong 1 Ca ShiftWork, nên trả về 1 ShiftWorkDto tổng hợp tất cả các số xe, doanh thu, chiết khấu, công nợ... của tất cả các xe chạy trong ca làm việc đó
    public static ShiftWorkDto ToDto(this List<ShiftWork> entities)
    {
        if (entities == null || entities.Count == 0)
            return null!; // hoặc throw exception, tùy yêu cầu

        // Lấy thông tin chung từ bản ghi đầu tiên
        var first = entities.First();
        
        return new ShiftWorkDto
        {
            Id = first.Id, // Sử dụng ID đầu tiên
            numberCar = entities //Lấy tất cả các số xe trong ca làm việc
                        .Where(x => !string.IsNullOrWhiteSpace(x.numberCar))
                        .Select(x => x.numberCar)
                        .Distinct() // loại trùng lặp nếu cần
                        .DefaultIfEmpty()
                        .Aggregate((a, b) => $"{a}, {b}")!,
            userId = first.userId,
            revenueByMonth = first.revenueByMonth + entities.Sum(x => x.revenueByDate ?? 0) ?? 0, //Doanh thu có sẳn + với doanh thu cộng dồn của các ngày trong ca làm việc (1 Tài - n Xe) 
            revenueByDate = entities.Sum(x => x.revenueByDate ?? 0), // Cộng dồn
            //qrContext = first.qrContext, // Không dùng trả về rỗng
            qrUrl = first.qrUrl,
            discountOther = entities.Sum(x => x.discountOther ?? 0), // Cộng dồn 
            arrearsOther = entities.Sum(x => x.arrearsOther ?? 0), // Cộng dồn
            totalPrice = entities.Sum(x => x.totalPrice ?? 0), // Cộng dồn
            walletGSM = entities.Sum(x => x.walletGSM ?? 0), // Cộng dồn
            discountGSM = entities.Sum(x => x.discountGSM ?? 0), // Cộng dồn
            discountNT = entities.Sum(x => x.discountNT ?? 0), //Cộng dồn 
            bank_Id = first.bank_Id,
            createdAt = first.createdAt,
            typeCar = first.typeCar,
            area = first.area,
            ranking = first.ranking,
            gasmoney = entities.Sum(x => x.gasmoney ?? 0), //Cộng dồn 
            pre_postpaid = entities.Sum(x => x.pre_postpaid ?? 0), //Cộng dồn 

            Trips = entities
                .SelectMany(x => x.Trips ?? new List<Trip>())
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    NumberCar = t.NumberCar,
                    tpDistance = t.tpDistance,
                    tpPrice = t.tpPrice,
                    tpPickUp = t.tpPickUp,
                    tpDropOut = t.tpDropOut,
                    tpType = t.tpType,
                    tpTimeStart = t.tpTimeStart,
                    tpTimeEnd = t.tpTimeEnd,
                    createdAt = t.createdAt
                }).OrderByDescending(e => e.tpTimeStart).ToList(),

            Contracts = entities
                .SelectMany(x => x.Contracts ?? new List<Contract>())
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
                    createdAt = c.createdAt
                }).ToList()
        };
    }



    // Convert ShiftWorkDto thành ShiftWork, cập nhật vào đối tượng ShiftWork đã tồn tại (dest)
    public static ShiftWork ToEntity(this ShiftWorkDto src, ShiftWork dest)
    {
        dest.numberCar = src.numberCar;
        dest.userId = src.userId;
        dest.revenueByMonth = src.revenueByMonth;
        dest.revenueByDate = src.revenueByDate;
        dest.qrContext = src.qrContext;
        dest.qrUrl = src.qrUrl;
        dest.discountOther = src.discountOther;
        dest.arrearsOther = src.arrearsOther;
        dest.totalPrice = src.totalPrice;
        dest.walletGSM = src.walletGSM;
        dest.discountGSM = src.discountGSM;
        dest.discountNT = src.discountNT;
        dest.bank_Id = src.bank_Id;
        dest.createdAt = src.createdAt;
        dest.typeCar = src.typeCar;
        dest.area = src.area;
        dest.ranking = src.ranking;
        dest.gasmoney = src.gasmoney;
        dest.pre_postpaid = src.pre_postpaid;

        return dest;
    }

    // Convert ShiftWorkDto thành ShiftWork mới, tạo đối tượng ShiftWork mới từ dữ liệu trong ShiftWorkDto
    public static ShiftWork ToEntity(this ShiftWorkDto src)
    {
        return new ShiftWork
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? Guid.NewGuid().ToString() : src.Id,
            numberCar = src.numberCar,
            userId = src.userId,
            revenueByMonth = src.revenueByMonth,
            revenueByDate = src.revenueByDate,
            qrContext = src.qrContext,
            qrUrl = src.qrUrl,
            discountOther = src.discountOther,
            arrearsOther = src.arrearsOther,
            totalPrice = src.totalPrice,
            walletGSM = src.walletGSM,
            discountGSM = src.discountGSM,
            discountNT = src.discountNT,
            bank_Id = src.bank_Id,
            createdAt = src.createdAt,
            typeCar = src.typeCar,
            area = src.area,
            ranking = src.ranking,
            gasmoney = src.gasmoney,
            pre_postpaid = src.pre_postpaid
        };
    }
}
