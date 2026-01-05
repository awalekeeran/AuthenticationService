using WebAPI.Models;

namespace WebAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> Authenticate(string username, string password);
        Task<User?> GetUserByUsername(string username);
        User GetUserByRefreshToken(string refreshToken);
    }
}
