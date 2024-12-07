using API.Models;

namespace API.DTO
{
    public class CoDriverDTO : Employee
    {
        public string LicenseNumber { get; set; }
        public int ExperienceYear { get; set; }
        public int BusRouteId { get; set; }
    }
}
