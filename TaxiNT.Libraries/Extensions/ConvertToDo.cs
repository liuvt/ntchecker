using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models.GGSheets;

namespace TaxiNT.Libraries.Extensions;

public static class ConvertToDo
{
    public static CheckerDetailDto ltvConvertCheckerDetailToDo(this CheckerDetailDto checkerDetail, Revenue _revenue, Timepiece _timepiece, GGSContract _contract)
    {
        //Convert revenuedetail to revenuedto
        var revenueDto = new RevenueDto
        {
            userId = _revenue.userId,
            qrUrl = _revenue.qrUrl,
            numberCar = _revenue.numberCar != null && _revenue.numberCar.Any() ? string.Join(", ", _revenue.numberCar) : string.Empty,
            revenueByMonth = _revenue.revenueByMonth.ltvVNDCurrencyToDecimal(),
            revenueByDate = _revenue.revenueByDate.ltvVNDCurrencyToDecimal(),
            discountOther = _revenue.discountOther.ltvVNDCurrencyToDecimal(),
            arrearsOther = _revenue.arrearsOther.ltvVNDCurrencyToDecimal(),
            totalPrice = _revenue.totalPrice.ltvVNDCurrencyToDecimal(),
            walletGSM = _revenue.walletGSM.ltvVNDCurrencyToDecimal(),
            discountGSM = _revenue.discountGSM.ltvVNDCurrencyToDecimal(),
            discountNT = _revenue.discountNT.ltvVNDCurrencyToDecimal(),
            createdAt = _revenue.createdAt,
            typeCar = _revenue.typeCar,
            area = _revenue.area,
            rank = _revenue.rank,
        };

        //LinQ convert timepiece to timepieceDto
        var timepieceDto = _timepiece.count > 0
            ? _timepiece.timepieces.Select(item => new TimepieceDto
            {
                userId = item.userId,
                numberCar = item.numberCar,
                tpTimeStart = item.tpTimeStart,
                tpTimeEnd = item.tpTimeEnd,
                tpDistance = item.tpDistance,
                tpPrice = item.tpPrice.ltvVNDCurrencyToDecimal(),
                tpPickUp = item.tpPickUp,
                tpDropOut = item.tpDropOut,
                tpType = item.tpType,
                createdAt = item.createdAt
            }).ToList()
            : new List<TimepieceDto>();

        //LinQ convert contract to contractDto
        var contractDto = _contract.count > 0
            ? _contract.contracts.Select(item => new ContractDto
            {
                userId = item.userId,
                numberCar = item.numberCar,
                ctKey = item.ctKey,
                ctDefaultAmount = item.ctAmount.ltvVNDCurrencyToDecimal(),
                ctDefaultDistance = item.ctDefaultDistance,
                ctOverDistance = item.ctOverDistance,
                ctSurcharge = item.ctSurcharge.ltvVNDCurrencyToDecimal(),
                ctPromotion = item.ctPromotion.ltvVNDCurrencyToDecimal(),
                ctTotalPrice = item.totalPrice.ltvVNDCurrencyToDecimal(),
                createdAt = item.createdAt
            }).ToList()
            : new List<ContractDto>();

        return new CheckerDetailDto
        {
            userId = revenueDto.userId,
            revenue = revenueDto,
            timepieces = timepieceDto,
            contracts = contractDto,
            countContract = _contract.count,
            TotalPriceContract = _contract.TotalPrice.ltvVNDCurrencyToDecimal(),
            countTimepices = _timepiece.count,
            TotalPriceTimepices = _timepiece.TotalPrice.ltvVNDCurrencyToDecimal()
        };
    }
}