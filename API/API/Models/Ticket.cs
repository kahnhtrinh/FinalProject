namespace API.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public Trip Trip { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
