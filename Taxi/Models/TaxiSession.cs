namespace Taxi.Models
{
    public class TaxiSession
    {
        public int Id { get; set; }
        public int DriverId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double DistanceTraveled { get; set; }
    }
}
