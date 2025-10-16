using TaxiNT.Libraries.Models;

namespace TaxiNT.Libraries.Entities
{
    public class ShiftWorkDailySyncDto
    {
        public List<ShiftWorkGroupDto> ShiftWorks { get; set; } = new();
    }

    public class ShiftWorkGroupDto
    {
        public ShiftWork ShiftWork { get; set; } = new();
        public List<TripDetail> Trips { get; set; } = new();
        public List<ContractDetail> Contracts { get; set; } = new();
    }

    public class ShiftWorkDto
    {
        public string Id { get; set; } = string.Empty;
        public string numberCar { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public decimal? revenueByMonth { get; set; }
        public decimal? revenueByDate { get; set; }

        public string qrContext { get; set; } = string.Empty;
        public string qrUrl { get; set; } = string.Empty;

        public decimal? discountOther { get; set; }
        public decimal? arrearsOther { get; set; }

        public decimal? totalPrice { get; set; }

        public decimal? walletGSM { get; set; }
        public decimal? discountGSM { get; set; }
        public decimal? discountNT { get; set; }

        public string bank_Id { get; set; } = string.Empty;
        public DateTime? createdAt { get; set; }
        public string typeCar { get; set; } = string.Empty;

        public string Area { get; set; } = string.Empty;
        public int Rank { get; set; }
        public decimal SauMucAnChia { get; set; } //Không dùng

        // Dữ liệu chi tiết
        public List<TripDto> Trips { get; set; } = new();
        public List<ContractDto> Contracts { get; set; } = new();

        // Tổng hợp
        public int TotalTrips => Trips.Count;
        public int TotalContracts => Contracts.Count;
        public decimal TotalTripPrice => Trips.Sum(t => t.tpPrice ?? 0);
        public decimal TotalContractPrice => Contracts.Sum(c => c.totalPrice ?? 0);
    }
}
