using WebAPI.Interfaces;
using WebAPI.Models;

namespace WebAPI.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly DataContext dc;

        public UserRespository(DataContext dc)
        {
            this.dc = dc;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            return null;// await dc.Users.FirstOrDefaultAsync(x=>x.UserName == userName && x.Password == password);
        }
    }
}
