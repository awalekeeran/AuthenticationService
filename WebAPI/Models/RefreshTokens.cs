using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class RefreshTokens
    {
        public int ID { get; set; }

        public int UserId { get; set; }

        public int TokenId { get; set; }

        public string RefreshToken { get; set; }

        public bool IsActive { get; set; }
    }
}
