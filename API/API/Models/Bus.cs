using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Bus
    {
        public int Id { get; set; }
        
        public string BusNumber { get; set; }
        
        public DateTime BeginDate { get; set; }
        
        public string Status { get; set; }
        public int BusRouteId { get; set; }
        
        public BusRoute BusRoute { get; set; }

        public ICollection<Trip> Trips { get; set; }
    }
}
