namespace API.Models
{
    public class CoDriver : Employee
    {
        public string LicenseNumber { get; set; }
        public int ExperienceYear { get; set; }
        public int BusRouteId { get; set; }
        public BusRoute BusRoute { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }
}
