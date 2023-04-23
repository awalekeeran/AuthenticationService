using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly DataContext dataContext;

        public UserRespository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<User> Authenticate(string userName, string password)
        {
            return await dataContext.Users.FirstOrDefaultAsync(x=>x.UserName == userName && x.Password == password);
        }
    }
}
