namespace WebAPI.DTOs
{
    public class LoginResDTO
    {
        public string UserName { get; set; }

        public string Token { get; set; }

        public string Expires { get; set; }

        public string RefreshToken { get; set; }
    }
}
