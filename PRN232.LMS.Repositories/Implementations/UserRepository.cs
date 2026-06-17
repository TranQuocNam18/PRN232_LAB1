using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly LmsDbContext _context;

        public UserRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByIdAsync(int id)
            => await _context.Users.FindAsync(id);

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
            => await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var rt = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (rt != null)
            {
                rt.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
