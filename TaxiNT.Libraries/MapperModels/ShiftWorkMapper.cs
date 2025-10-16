using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Libraries.MapperModels
{
    public static class ShiftWorkMapper
    {
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
                revenueByMonth = first.revenueByMonth + entities.Sum(x => x.revenueByDate ?? 0) ?? 0, // Không cộng dồn vì cả 2 đều = nhau 
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
                Area = first.Area,
                Rank = first.Rank,
                SauMucAnChia = entities.Sum(x => x.SauMucAnChia), // Cộng dồn nhưng sai vì lương ngày phải tính lại lương

                Trips = entities
                    .SelectMany(x => x.Trips ?? new List<TripDetail>())
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
                    .SelectMany(x => x.Contracts ?? new List<ContractDetail>())
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
    }
}
