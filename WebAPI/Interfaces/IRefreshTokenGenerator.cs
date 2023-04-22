namespace WebAPI.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken(int userId);
    }
}
