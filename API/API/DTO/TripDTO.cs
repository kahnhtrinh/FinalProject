namespace API.Models
{
    public class TripDTO
    {
        public int Id { get; set; }
        public int? BusRouteId { get; set; } = 1;
        public int BusId { get; set; }
        public int DriverId { get; set; }
        public int CoDriverId { get; set; }
        public TimeSpan? Duration { get; set; } = TimeSpan.Parse("05:30:00");
        public DateTime DepartureDate { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Chưa chạy";
    }
}
