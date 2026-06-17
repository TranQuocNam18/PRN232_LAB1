namespace PRN232.LMS.Repositories.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;
        public User User { get; set; } = null!;
    }
}
