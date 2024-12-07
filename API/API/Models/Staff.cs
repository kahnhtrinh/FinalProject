using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Staff : Employee
    {
        public string EmailAddress { get; set; }
        public Account Account   { get; set; }
        public int AccountId { get; set; }
    }
}
