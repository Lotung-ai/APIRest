using Moq;
using Xunit;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Models;
using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Http;

namespace P7CreateRestApi.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole<int>>> _mockRoleManager;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );
            _mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                new Mock<IRoleStore<IdentityRole<int>>>().Object,
                null,
                null,
                null,
                null
            );
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(
                _mockUserRepository.Object,
                _mockUserManager.Object,
                _mockRoleManager.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Model is invalid");
            var registerModel = new RegisterModel { UserName = "test", Email = "test@example.com", Password = "Password123", Fullname = "Test User", Role = "User" };

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            // Vérifie que le résultat est un BadRequestObjectResult lorsque le modèle est invalide.
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            // Vérifie que le code de statut HTTP est 400.
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Register_CreatesRole_WhenRoleDoesNotExist()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "test", Email = "test@example.com", Password = "Password123", Fullname = "Test User", Role = "User" };
            _mockRoleManager.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(false);
            _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<int>>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _controller.Register(registerModel);

            // Assert
            // Vérifie que CreateAsync a été appelé une fois avec un rôle ayant le bon nom.
            _mockRoleManager.Verify(r => r.CreateAsync(It.Is<IdentityRole<int>>(role => role.Name == registerModel.Role)), Times.Once);
        }

        [Fact]
        public async Task Register_ReturnsServerError_WhenRoleCreationFails()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "test",
                Email = "test@example.com",
                Password = "Password123",
                Fullname = "Test User",
                Role = "User"
            };
            _mockRoleManager.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(false);
            _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<int>>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role creation failed" }));
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            // Vérifie que le résultat est un ObjectResult avec un code de statut HTTP 500 lorsque la création du rôle échoue.
            Assert.IsType<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.NotNull(objectResult);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            // Vérifie que le message d'erreur dans le corps de la réponse est correct.
            Assert.Equal("Unable to create role.", objectResult.Value);
        }

        [Fact]
        public async Task Register_ReturnsCreatedAtAction_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "test", Email = "test@example.com", Password = "Password123", Fullname = "Test User", Role = "User" };
            var user = new User { Id = 1, UserName = registerModel.UserName, Email = registerModel.Email, Fullname = registerModel.Fullname, Role = registerModel.Role };
            _mockRoleManager.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(false);
            _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<int>>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.AddToRoleAsync(user, registerModel.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            // Vérifie que le résultat est un CreatedAtActionResult lorsque l'utilisateur est créé avec succès.
            Assert.IsType<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            // Vérifie que le code de statut HTTP est 201.
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserCreationFails()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "test", Email = "test@example.com", Password = "Password123", Fullname = "Test User", Role = "User" };
            _mockRoleManager.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(false);
            _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<int>>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), registerModel.Password)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            // Vérifie que le résultat est un BadRequestObjectResult lorsque la création de l'utilisateur échoue.
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            // Vérifie que le code de statut HTTP est 400.
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Register_AddsUserToRole_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                UserName = "test",
                Email = "test@example.com",
                Password = "Password123",
                Fullname = "Test User",
                Role = "User"
            };
            var user = new User
            {
                Id = 1,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                Fullname = registerModel.Fullname,
                Role = registerModel.Role
            };

            // Setup RoleManager to indicate that the role does not exist
            _mockRoleManager.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(false);

            // Setup RoleManager to create a new role successfully
            _mockRoleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<int>>())).ReturnsAsync(IdentityResult.Success);

            // Setup UserManager to create the user successfully
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<User>(), registerModel.Password)).ReturnsAsync(IdentityResult.Success);

            // Setup UserManager to add the user to a role successfully
            _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), registerModel.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _controller.Register(registerModel);

            // Assert
            // Vérifie que AddToRoleAsync a été appelé une fois avec l'utilisateur et le rôle corrects.
            _mockUserManager.Verify(u => u.AddToRoleAsync(It.Is<User>(user => user.UserName == registerModel.UserName), registerModel.Role), Times.Once);
        }

        [Fact]
        public async Task GetUserById_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, UserName = "test", Email = "test@example.com", Fullname = "Test User", Role = "User" };
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            // Vérifie que le résultat est un OkObjectResult lorsque l'utilisateur est trouvé.
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            // Vérifie que le code de statut HTTP est 200.
            Assert.Equal(200, okResult.StatusCode);
            // Vérifie que la valeur de la réponse est l'utilisateur attendu.
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            // Vérifie que le résultat est un NotFoundResult lorsque l'utilisateur n'est pas trouvé.
            Assert.IsType<NotFoundResult>(result);
            var notFoundResult = result as NotFoundResult;
            Assert.NotNull(notFoundResult);
            // Vérifie que le code de statut HTTP est 404.
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, UserName = "user1", Email = "user1@example.com", Fullname = "User One", Role = "User" },
                new User { Id = 2, UserName = "user2", Email = "user2@example.com", Fullname = "User Two", Role = "User" }
            };
            _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            // Vérifie que le résultat est un OkObjectResult contenant la liste des utilisateurs.
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            // Vérifie que le code de statut HTTP est 200.
            Assert.Equal(200, okResult.StatusCode);
            // Vérifie que la valeur de la réponse est la liste des utilisateurs attendue.
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WithEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<User>();
            _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            // Vérifie que le résultat est un OkObjectResult avec une liste d'utilisateurs vide.
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            // Vérifie que le code de statut HTTP est 200.
            Assert.Equal(200, okResult.StatusCode);
            // Vérifie que la valeur de la réponse est une liste vide.
            Assert.Empty(okResult.Value as List<User>);
        }
    }
}
