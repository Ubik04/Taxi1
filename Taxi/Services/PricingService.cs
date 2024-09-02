namespace Taxi.Services
{
    public class PricingService
    {
        private readonly double _ratePerKm;
        private readonly double _ratePerHour;

        public PricingService(double ratePerKm, double ratePerHour)
        {
            _ratePerKm = ratePerKm;
            _ratePerHour = ratePerHour;
        }

        public double CalculateTotalCost(DateTime startTime, DateTime endTime, double distanceTraveled)
        {
            if (endTime == null) throw new ArgumentException("Session is not ended");

            var durationInHours = (endTime - startTime).TotalHours;
            var distanceCost = distanceTraveled * _ratePerKm;
            var timeCost = durationInHours * _ratePerHour;
            return distanceCost + timeCost;
        }
    }
}
