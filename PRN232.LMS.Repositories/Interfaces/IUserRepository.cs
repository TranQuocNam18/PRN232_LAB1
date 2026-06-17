using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeRefreshTokenAsync(string token);
    }
}
