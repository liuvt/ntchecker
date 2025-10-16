
namespace TaxiNT.Libraries.Models.Zalos
{
    public class ZPayloads
    {
        public ZCustomer customer { get; set; } = new ZCustomer();
        public List<ZItem> attrs { get; set; } = new List<ZItem>();
    }
}
