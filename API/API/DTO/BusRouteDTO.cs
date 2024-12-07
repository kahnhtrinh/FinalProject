using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class BusRouteDTO
    {
        [Required]
        public string Departure { get; set; }
        [Required]
        public string Destionation { get; set; }
        [Required]
        public string Distance { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public string Price { get; set; }
        [Required]
        public string Available { get; set; }
    }
}
