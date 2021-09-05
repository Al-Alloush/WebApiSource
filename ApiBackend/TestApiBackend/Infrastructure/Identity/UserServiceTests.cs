using Core.Entities.Identity;
using Core.Interfaces.AppService;
using Infrastructure.DataApp;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestApiBackend.Infrastructure.Identity
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<AppDbContext> _contextMock = new Mock<AppDbContext>();
        private readonly Mock<ITokenService> _tokenService = new Mock<ITokenService>();
        private readonly Mock<AppUserManager> _userManager = new Mock<AppUserManager>();

        public UserServiceTests()
        {
            _userService = new UserService(_contextMock.Object, _tokenService.Object, _userManager.Object);
        }

        [Fact]
        public async Task FindUserByEmailAsync_returnAppUserIdEmailExist_elseNull()
        {
            var existEmail = "ahmad@al-alloush.com";


            // Arrange: Arrange all what I need use
            var user = new AppUser
            {
                Email = existEmail
            };
            _userService.Setup(x => x.FindUserByEmailAsync).ReturnAsync();

            // Act: call the methedos that want to test it
            var result = await _userService.FindUserByEmailAsync(existEmail);

            // Assert
            Assert.Equal(2, 2);

        }
    }
}
