using API.Models;

namespace API.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public ICollection<string> SeatNames { get; set; }
    }
}
