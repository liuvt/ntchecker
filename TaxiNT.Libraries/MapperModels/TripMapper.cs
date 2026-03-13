using TaxiNT.Libraries.Entities;
using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.MapperModels;

public static class TripMapper
{
    public static Trip TripDtoToEntity(this TripDto src, string shiftworkId)
    {
        return new Trip
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? Guid.NewGuid().ToString() : src.Id,
            NumberCar = src.NumberCar,
            tpTimeStart = src.tpTimeStart,
            tpTimeEnd = src.tpTimeEnd,
            tpDistance = src.tpDistance,
            tpPrice = src.tpPrice,
            tpPickUp = src.tpPickUp,
            tpDropOut = src.tpDropOut,
            tpType = src.tpType,
            userId = src.userId,
            createdAt = src.createdAt,
            shiftworkId = shiftworkId
        };
    }
}
