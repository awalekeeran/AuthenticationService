using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly DataContext dataContext;
        private readonly IPasswordHasher passwordHasher;

        public UserRespository(DataContext dataContext, IPasswordHasher passwordHasher)
        {
            this.dataContext = dataContext;
            this.passwordHasher = passwordHasher;
        }

        public async Task<User?> Authenticate(string userName, string password)
        {
            var user = await GetUserByUsername(userName);
            
            if (user == null)
            {
                return null;
            }

            // Verify the password against the stored hash
            bool isPasswordValid = passwordHasher.VerifyPassword(password, user.PasswordHash);
            
            return isPasswordValid ? user : null;
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            return dataContext.Users.FirstOrDefault(x => x.RefreshToken == refreshToken && x.IsActive);
        }
    }
}
