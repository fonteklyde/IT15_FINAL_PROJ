using Microsoft.AspNetCore.Mvc;

namespace IT15_Final_Proj.Models
{
    public class LoginLog
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
