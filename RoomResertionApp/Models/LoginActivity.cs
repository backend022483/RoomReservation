namespace RoomResertionApp.Models
{
    public class LoginActivity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserRole { get; set; }
    }
}
