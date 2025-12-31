using WebAPI.Interfaces;
using WebAPI.Repositories;

namespace WebAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext dataContext;
        private readonly IPasswordHasher passwordHasher;

        public UnitOfWork(DataContext dataContext, IPasswordHasher passwordHasher)
        {
            this.dataContext = dataContext;
            this.passwordHasher = passwordHasher;
        }

        public ICityRepository CityRepository => new CityRepository(dataContext);

        public IUserRepository UserRepository => new UserRespository(dataContext, passwordHasher);

        public IRefreshTokenGenerator RefreshTokenGeneratorRepository => new RefreshTokenGeneratorRepository(dataContext);

        public async Task<bool> SaveAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}
