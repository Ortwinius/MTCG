using Microsoft.AspNetCore.Identity;
using MTCG.BusinessLogic.Services;
using MTCG.Models.Users;
using MTCG.Repositories.Interfaces;
using MTCG.Utilities.Exceptions.CustomExceptions;
using NSubstitute;
using NUnit.Framework;

namespace MTCGTests.UserTests
{
    [TestFixture]
    public class RegistrationAndLoginTest
    {
        private AuthService _authService;
        private IUserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            AuthService.ResetInstance();
            _userRepository = Substitute.For<IUserRepository>();
            _authService = AuthService.GetInstance(_userRepository);
        }

        [Test]
        public void Register_UserDoesNotExist_ShouldRegisterSuccessfully()
        {
            
            var username = "dasddassad";
            var password = "asdasdasda";

            _userRepository.UserExists(username).Returns(false); // user doesnt exist

            
            _authService.Register(username, password);

             // assert that password was hashed and authtoken was generated
            _userRepository.Received(1).AddUser(Arg.Is<User>(u =>
                u.Username == username &&
                !string.IsNullOrWhiteSpace(u.Password) && 
                !string.IsNullOrWhiteSpace(u.AuthToken))); 
        }


        [Test]
        public void Login_UserExistsAndPasswordIsCorrect_ShouldLoginSuccessfully()
        {
            
            var username = "dasddassad";
            var password = "asdasdasda";

            // create user and hash password
            var user = new User(username, string.Empty);
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, password);

            _userRepository.GetUserByUsername(username).Returns(user);

            var authToken = string.Empty;

            
            Assert.DoesNotThrow(() => _authService.Login(username, password, out authToken));

            
            Assert.IsFalse(string.IsNullOrWhiteSpace(authToken)); // authtoken is generated
            _userRepository.Received(1).UpdateUser(Arg.Is<User>(u => u.AuthToken == authToken)); // ensure that UpdateUser gets called
        }

    }
}
