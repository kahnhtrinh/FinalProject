using API.Models;

namespace API.DTO
{
    public class StaffDTO : Employee
    {
        public string EmailAddress { get; set; }
        public int AccountId { get; set; }
    }
}
