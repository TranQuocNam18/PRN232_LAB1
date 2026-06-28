using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests.Auth;
using PRN232.LMS.Services.Models.Responses.Auth;

namespace PRN232.LMS.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null) return null;

            // Verify password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow) return null;

            // Revoke old token
            await _userRepository.RevokeRefreshTokenAsync(refreshToken.Token);

            // Generate new ones
            return await GenerateAuthResponseAsync(refreshToken.User);
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? "PRN232-LMS-SuperSecretKey-ChangeInProduction-AtLeast32Chars!";
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var jwtIssuer = _configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "PRN232.LMS.API";
            var jwtAudience = _configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "PRN232.LMS.Client";
            var expiryMinutesStr = _configuration["Jwt:ExpiryMinutes"] ?? "60";
            if (!double.TryParse(expiryMinutesStr, out var expiryMinutes))
            {
                expiryMinutes = 60;
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = jwtIssuer,
                Audience = jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Generate refresh token
            var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var newRefreshToken = new RefreshToken
            {
                Token = rawRefreshToken,
                UserId = user.UserId,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            await _userRepository.AddRefreshTokenAsync(newRefreshToken);

            return new AuthResponse
            {
                Token = tokenString,
                AccessToken = tokenString,
                RefreshToken = rawRefreshToken,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}
