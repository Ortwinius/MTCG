using MTCG.BusinessLogic.Services;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;
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
            
            var headers = new Dictionary<string, string> { { "Authorization", "valid-auth-token" } };

            var result = _authService.GetAuthToken(headers);

            Assert.AreEqual("valid-auth-token", result);
        }

        [Test]
        public void GetAuthToken_InvalidHeaders_ShouldThrowUnauthorizedException()
        {
            var headers = new Dictionary<string, string>(); // No "Authorization" key

            Assert.Throws<UnauthorizedException>(() => _authService.GetAuthToken(headers));
        }

        [Test]
        public void EnsureAuthenticated_ValidAuthToken_ShouldNotThrow()
        {
            var authToken = "valid-auth-token";
            _userRepository.ValidateToken(authToken).Returns(true);

            Assert.DoesNotThrow(() => _authService.EnsureAuthenticated(authToken));
        }

        [Test]
        public void EnsureAuthenticated_InvalidAuthToken_ShouldThrowUnauthorizedException()
        {
            var authToken = "invalid-auth-token";
            _userRepository.ValidateToken(authToken).Returns(false);

            Assert.Throws<UnauthorizedException>(() => _authService.EnsureAuthenticated(authToken));
        }

        [Test]
        public void EnsureAuthenticated_PathUsernameMismatch_ShouldThrowUnauthorizedException()
        {            
            var authToken = "user-mtcgToken";
            var pathUsername = "otherUser";

            var user = new User(pathUsername, "password") { AuthToken = "otherUser-mtcgToken" };
            _userRepository.GetUserByUsername(pathUsername).Returns(user);

            Assert.Throws<UnauthorizedException>(() => _authService.EnsureAuthenticated(authToken, pathUsername));
        }

        [Test]
        public void EnsureAuthenticated_ValidAdminAuthToken_ShouldNotThrow()
        {
            var authToken = "admin-mtcgToken";
            var adminUser = new User("admin", "password") { AuthToken = "admin-mtcgToken" };

            _userRepository.GetUserByToken(authToken).Returns(adminUser);

            Assert.DoesNotThrow(() => _authService.EnsureAuthenticated(authToken, allowAdmin: true));
        }
    }
}
