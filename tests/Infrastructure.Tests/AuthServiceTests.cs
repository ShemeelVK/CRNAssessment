using System;
using System.Threading.Tasks;
using CRNAssessment.Application.DTOs;
using CRNAssessment.Application.Interfaces;
using CRNAssessment.Domain.Entities;
using CRNAssessment.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CRNAssessment.Infrastructure.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();

            //setup IConfiguration mock for JWT settings
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.Setup(x => x["key"]).Returns("ThisIsAVerySecureKeyForTestingPurposesOnly123!");
            jwtSectionMock.Setup(x => x["DurationInMinutes"]).Returns("60");
            jwtSectionMock.Setup(x => x["Issuer"]).Returns("TestIssuer");
            jwtSectionMock.Setup(x => x["Audience"]).Returns("TestAudience");

            _configurationMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUsernameIsTaken()
        {
            var dto = new RegisterRequestDto { Username = "existinguser", Password = "password123" };
            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(dto.Username))
                .ReturnsAsync(new User { Username = dto.Username });

            Func<Task> action = async () => await _authService.RegisterAsync(dto);

            await action.Should().ThrowAsync<Exception>().WithMessage("Username is already taken.");
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenUsernameIsAvailable()
        {
            var dto = new RegisterRequestDto { Username = "newuser", Password = "password123" };
            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(dto.Username))
                .ReturnsAsync((User)null); // Username is available

            await _authService.RegisterAsync(dto);

            _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => 
                u.Username == dto.Username && 
                u.Role == "User" && 
                !string.IsNullOrEmpty(u.PasswordHash))), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
        {
            var dto = new LoginRequestDto { Username = "unknownuser", Password = "password123" };
            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(dto.Username))
                .ReturnsAsync((User)null);

            Func<Task> action = async () => await _authService.LoginAsync(dto);

            await action.Should().ThrowAsync<Exception>().WithMessage("Invalid username or password.");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            var dto = new LoginRequestDto { Username = "testuser", Password = "wrongpassword" };
            var existingUser = new User
            {
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(dto.Username))
                .ReturnsAsync(existingUser);

            Func<Task> action = async () => await _authService.LoginAsync(dto);

            await action.Should().ThrowAsync<Exception>().WithMessage("Invalid username or password.");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            var dto = new LoginRequestDto { Username = "testuser", Password = "correctpassword" };
            var existingUser = new User
            {
                Username = "testuser",
                Role = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
            };

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(dto.Username))
                .ReturnsAsync(existingUser);

            var response = await _authService.LoginAsync(dto);

            response.Should().NotBeNull();
            response.Token.Should().NotBeNullOrEmpty();
            response.RefreshToken.Should().NotBeNullOrEmpty();
            
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => 
                u.RefreshToken == response.RefreshToken && 
                u.RefreshTokenExpiry > DateTime.UtcNow)), Times.Once);
        }
    }
}
