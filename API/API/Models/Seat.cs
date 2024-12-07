namespace API.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public string SeatName { get; set; }
        public int? TicketId { get; set; }
        public string Status { get; set; }
        public Ticket Ticket { get; set; }
        public Trip Trip { get; set; }
    }
}
