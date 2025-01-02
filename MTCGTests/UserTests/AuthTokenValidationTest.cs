using MTCG.BusinessLogic.Services;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.CustomExceptions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MTCGTests.UserTests
{
    [TestFixture]
    public class AuthTokenValidationTest
    {
        private AuthService _authService;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            AuthService.ResetInstance(); // Reset Singleton
            _userRepository = Substitute.For<IUserRepository>();
            _authService = AuthService.GetInstance(_userRepository);
        }

        [Test]
        public void GetAuthToken_ValidHeaders_ShouldReturnAuthToken()
        {
            // Arrange
            var headers = new Dictionary<string, string> { { "Authorization", "valid-auth-token" } };

            // Act
            var result = _authService.GetAuthToken(headers);

            // Assert
            Assert.AreEqual("valid-auth-token", result);
        }

        [Test]
        public void GetAuthToken_InvalidHeaders_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var headers = new Dictionary<string, string>(); // No "Authorization" key

            // Act & Assert
            Assert.Throws<UnauthorizedException>(() => _authService.GetAuthToken(headers));
        }

        [Test]
        public void EnsureAuthenticated_ValidAuthToken_ShouldNotThrow()
        {
            // Arrange
            var authToken = "valid-auth-token";
            _userRepository.ValidateToken(authToken).Returns(true);

            // Act & Assert
            Assert.DoesNotThrow(() => _authService.EnsureAuthenticated(authToken));
        }

        [Test]
        public void EnsureAuthenticated_InvalidAuthToken_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var authToken = "invalid-auth-token";
            _userRepository.ValidateToken(authToken).Returns(false);

            // Act & Assert
            Assert.Throws<UnauthorizedException>(() => _authService.EnsureAuthenticated(authToken));
        }

        [Test]
        public void EnsureAuthenticated_PathUsernameMismatch_ShouldThrowUnauthorizedException()
        {
            // Arrange
            
            var authToken = "user-mtcgToken";
            var pathUsername = "otherUser";

            var user = new User(pathUsername, "password") { AuthToken = "otherUser-mtcgToken" };
            _userRepository.GetUserByUsername(pathUsername).Returns(user);

            // Act & Assert
            Assert.Throws<UnauthorizedException>(() => _authService.EnsureAuthenticated(authToken, pathUsername));
        }

        [Test]
        public void EnsureAuthenticated_ValidAdminAuthToken_ShouldNotThrow()
        {
            // Arrange
            var authToken = "admin-mtcgToken";
            var adminUser = new User("admin", "password") { AuthToken = "admin-mtcgToken" };

            _userRepository.GetUserByToken(authToken).Returns(adminUser);

            // Act & Assert
            Assert.DoesNotThrow(() => _authService.EnsureAuthenticated(authToken, allowAdmin: true));
        }
    }
}
