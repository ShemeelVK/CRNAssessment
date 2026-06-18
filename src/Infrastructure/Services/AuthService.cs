using CRNAssessment.Application.DTOs;
using CRNAssessment.Application.Interfaces;
using CRNAssessment.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CRNAssessment.Infrastructure.Services
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

        public async Task RegisterAsync(RegisterRequestDto request)
        {
            //check if username is already taken
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new Exception("Username is already taken.");
            }

            //hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //create the user
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = "User"
            };
            await _userRepository.AddAsync(newUser);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid username or password.");

            //generate JWT Token
            var Accesstoken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return new LoginResponseDto { Token = Accesstoken, RefreshToken = refreshToken };
        }

        private string GenerateJwtToken(User user)
        {
            //secret key settings from appsettings.json
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // data embedded inside the token (Username and Role)
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["DurationInMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokendescriptor);

            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
   
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token.");


            var newAccessToken = GenerateJwtToken(user);

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);
            return new LoginResponseDto { Token = newAccessToken, RefreshToken = newRefreshToken };
        }
    }
}
