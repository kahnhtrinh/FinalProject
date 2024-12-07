namespace API.Models
{
    public class Trip
    {
        public int Id { get; set; }
        public BusRoute BusRoute { get; set; }
        public Driver Driver { get; set; }
        public CoDriver CoDriver { get; set; }
        public int? BusRouteId { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }

        public int BusId { get; set; }

        public Bus Bus { get; set; }
        public int DriverId { get; set; }
        public int CoDriverId { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
    }
}
