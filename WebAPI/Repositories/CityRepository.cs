using WebAPI.Data;
using WebAPI.Interfaces;

namespace WebAPI.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly DataContext dataContext;

        public CityRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
    }
}
