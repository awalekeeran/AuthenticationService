using WebAPI.Interfaces;
using WebAPI.Repositories;

namespace WebAPI.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext dataContext;

        public UnitOfWork(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public ICityRepository CityRepository => new CityRepository(dataContext);

        public IUserRepository UserRepository => new UserRespository(dataContext);

        public IRefreshTokenGenerator RefreshTokenGeneratorRepository => new RefreshTokenGeneratorRepository(dataContext);

        public async Task<bool> SaveAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }
    }
}
