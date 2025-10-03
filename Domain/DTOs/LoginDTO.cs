namespace Domain.DTOs
{
    public class LoginDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }
    }
}
