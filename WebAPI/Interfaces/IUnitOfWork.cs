namespace WebAPI.Interfaces
{
    public interface IUnitOfWork
    {
        ICityRepository CityRepository { get; }

        IUserRepository UserRepository { get; }

        IRefreshTokenGenerator RefreshTokenGeneratorRepository { get; }

        Task<bool> SaveAsync();
    }
}
