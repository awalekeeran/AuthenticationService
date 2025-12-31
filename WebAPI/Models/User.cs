using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class User
    {
        public int ID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string RefreshToken { get; set; }

        public bool IsActive { get; set; }
    }
}
