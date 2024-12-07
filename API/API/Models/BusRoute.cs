using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace API.Models
{
    [Table("busroutes")]
    public class BusRoute
    {
        public int Id { get; set; }
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
        [Required]
        public ICollection<Bus> Buses { get; set; }

        [Required]
        public ICollection<Trip> Trips { get; set; }
        [Required]
        public ICollection<Driver> Drivers { get; set; }
        [Required]
        public ICollection<CoDriver> CoDrivers { get; set; }
    }
}
