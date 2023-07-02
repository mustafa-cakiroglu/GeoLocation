using Nest;

namespace GeoLocationTest.Models
{
    public class CustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public GeoLocation Location { get; set; }
        public double DistanceKm { get; set; }
    }
}
