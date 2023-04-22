using System.Security.Cryptography;
using WebAPI.Data;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class RefreshTokenGeneratorRepository : IRefreshTokenGenerator
    {
        private readonly DataContext dataContext;

        public RefreshTokenGeneratorRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public string GenerateToken(int userId)
        {
            var randomNumber = new byte[32];

            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);

                string refreshToken = Convert.ToBase64String(randomNumber);

                var user = dataContext.Users.FirstOrDefault(o=>o.ID== userId);

                if (user != null)
                {
                    user.RefreshToken= refreshToken;
                    dataContext.SaveChanges();
                }
                else
                {
                    RefreshTokens refreshTokens = new RefreshTokens
                    {
                        UserId = userId,
                        TokenId = new Random().Next(),
                        RefreshToken = refreshToken,
                        IsActive= true
                    };

                    dataContext.SaveChanges();
                }

                return refreshToken;
            }
        }
    }
}