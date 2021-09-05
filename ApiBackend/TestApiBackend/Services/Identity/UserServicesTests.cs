//using Core.Entities.Identity;
//using Core.Interfaces.AppService;
//using Core.Interfaces.AppService.RepositoriesAndPatterns;
//using Core.SpecificationsQueries.Identity;
//using Infrastructure.DataApp;
//using Infrastructure.Services.Identity;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace TestApiBackend.Services.Identity
//{
//    public class UserServicesTests
//    {

//        private readonly UserService _userService;
//        private readonly Mock<IUnitOfWork> _unitOfWorkMoq = new Mock<IUnitOfWork>();
//        private readonly Mock<ITokenService> _tokenServiceMoq = new Mock<ITokenService>();
        
//        public AppUser USERError;
//        public AppUser USER;
//        private string USER_EMAILNotExist = "Jack@outlook.de";
//        private string USER_EMAIL = "ahmad@gsmail.com";
//        private string USER_USERNAME = "Alloush";



//        public UserServicesTests()
//        {
//            _userService = new UserService(_unitOfWorkMoq.Object, null, _tokenServiceMoq.Object, null);
//            InitiAppUsers();
//        }

//        private void InitiAppUsers()
//        {
//            var userSuccess = new AppUser
//            {
//                Email = USER_EMAIL,
//                UserName = USER_USERNAME,

//            };

//            USER = userSuccess;

//            var userError = new AppUser
//            {
//                Email = USER_EMAILNotExist,
//                UserName = "JAck",

//            };

//            USERError = userError;
//        }

//        [Fact]
//        public async Task FindUserByEmailAsync_ReturnUserIfExist_ElseNull()
//        {
//            // Arrange: Arrange all what I need use
//            var speci = new SpeciUser_FindUserByEmail(USER_EMAIL);
//            _unitOfWorkMoq.Setup(x => x.Repository<AppUser>().EntityAsync(speci)).ReturnsAsync(USER);

//            // Act: call the methedos that want to test it

//            var findUser = await _userService.FindUserByEmailAsync(speci);

//            // Assert : check 

//            Assert.Equal(findUser, USER);
//            Assert.NotEqual(findUser, USERError);

//        }
//    }
//}
