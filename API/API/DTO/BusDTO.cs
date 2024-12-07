using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class BusDTO
    {
        public int Id { get; set; }
 
        public string BusNumber { get; set; }
       
        public DateTime BeginDate { get; set; }

        public string Status { get; set; }
        
        public int BusRouteId { get; set; }
    }
}
