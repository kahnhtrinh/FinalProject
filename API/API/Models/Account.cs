namespace API.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Role { get; set; }
        public Staff Staff { get; set; }
        public int? StaffId { get; set; }
    }
}
