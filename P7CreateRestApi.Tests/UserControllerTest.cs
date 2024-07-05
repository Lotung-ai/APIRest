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
using System.Net;
using Newtonsoft.Json.Linq;

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


        // Test pour UpdateUser - La mise à jour réussit
        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            // Crée un utilisateur original avec des données spécifiques.
            var originalUser = new User
            {
                Id = 1,
                UserName = "OldAccount",
                Email = "old.email@example.com",
                Fullname = "OldFullname",
                Role = "OldRole"
            };

            // Crée un modèle de mise à jour avec de nouvelles données.
            var updatedUser = new RegisterModel
            {
                UserName = "NewAccount",
                Email = "new.email@example.com",
                Password = "NewPassword",
                Fullname = "NewFullname",
                Role = "NewRole"
            };

            // Configure les mocks pour renvoyer l'utilisateur original et simuler une mise à jour réussie.
            _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync(originalUser);
            _mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            _mockRoleManager.Setup(rm => rm.RoleExistsAsync(updatedUser.Role)).ReturnsAsync(true);

            // Act
            // Appelle la méthode UpdateUser du contrôleur pour effectuer la mise à jour.
            var result = await _controller.UpdateUser(1, updatedUser);

            // Assert
            // Vérifie que le résultat est un OkObjectResult, ce qui signifie que la mise à jour a réussi.
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Vérifie que la réponse contient un utilisateur mis à jour avec les nouvelles données.
            var user = Assert.IsType<User>(okResult.Value);
            Assert.Equal("NewAccount", user.UserName);
            Assert.Equal("new.email@example.com", user.Email);
            Assert.Equal("NewFullname", user.Fullname);
            Assert.Equal("NewRole", user.Role);

        }

        // Test pour UpdateUser - L'élément à mettre à jour n'existe pas
        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            // Crée un modèle de mise à jour avec de nouvelles données.
            var user = new RegisterModel
            {
                UserName = "NewAccount",
                Email = "new.email@example.com",
                Password = "NewPassword",
                Fullname = "NewFullname",
                Role = "NewRole"
            };

            // Configure le mock pour renvoyer null pour la recherche d'utilisateur, simulant un utilisateur non trouvé.
            _mockUserManager.Setup(um => um.FindByIdAsync("1")).ReturnsAsync((User)null);

            // Act
            // Appelle la méthode UpdateUser du contrôleur pour essayer de mettre à jour un utilisateur qui n'existe pas.
            var result = await _controller.UpdateUser(1, user);

            // Assert
            // Vérifie que le résultat est un NotFoundResult, ce qui signifie que l'utilisateur n'a pas été trouvé.
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        // Test pour UpdateUser - Une exception est levée
        [Fact]
        public async Task UpdateUser_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            // Crée un modèle de mise à jour avec de nouvelles données.
            var registerModel = new RegisterModel
            {
                UserName = "NewAccount",
                Email = "new.email@example.com",
                Password = "NewPassword",
                Fullname = "NewFullname",
                Role = "NewRole"
            };

            // Configure le mock pour lancer une exception lors de la recherche d'un utilisateur, simulant une erreur de base de données.
            _mockUserManager.Setup(um => um.FindByIdAsync("1")).ThrowsAsync(new Exception("Database error"));

            // Act
            // Appelle la méthode UpdateUser du contrôleur pour tester le comportement en cas d'exception.
            var result = await _controller.UpdateUser(1, registerModel);

            // Assert
            // Vérifie que le résultat est un ObjectResult, ce qui signifie que le serveur a rencontré une erreur interne.
            var actionResult = Assert.IsType<ObjectResult>(result);

            // Vérifie que le code de statut HTTP est 500 (Internal Server Error), indiquant une erreur interne.
            Assert.Equal(StatusCodes.Status500InternalServerError, actionResult.StatusCode);

            // Vérifie que le message d'erreur est "Internal server error".
            Assert.Equal("Internal server error", actionResult.Value);
        }

        // Test pour DeleteUser - Suppression réussie
        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            int userId = 1;
            var user = new User { Id = userId };

            // Configure le mock pour retourner un utilisateur existant et simuler une suppression réussie.
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);  // Simule la présence de l'utilisateur
            _mockUserRepository.Setup(r => r.DeleteUserAsync(userId))
                .ReturnsAsync(true);  // Simule une suppression réussie

            // Act
            // Appelle la méthode DeleteUser du contrôleur pour effectuer la suppression.
            var result = await _controller.DeleteUser(userId);

            // Assert
            // Vérifie que le résultat est un NoContentResult, ce qui signifie que la suppression a été effectuée avec succès.
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteUser - User non trouvé
        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 1;

            // Configure le mock pour simuler que l'utilisateur n'existe pas, donc la suppression échoue.
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            // Appelle la méthode DeleteUser du contrôleur pour essayer de supprimer un utilisateur qui n'existe pas.
            var result = await _controller.DeleteUser(userId);

            // Assert
            // Vérifie que le résultat est un NotFoundResult, ce qui signifie que l'utilisateur n'a pas été trouvé.
            Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, (result as NotFoundResult).StatusCode);
        }

        // Test pour DeleteUser - Exception levée
        [Fact]
        public async Task DeleteUser_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int userId = 1;
            var expectedMessage = "Internal server error";

            // Configure le mock pour lancer une exception lors de la suppression, simulant une erreur de base de données.
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });  // Simule un utilisateur existant
            _mockUserRepository.Setup(r => r.DeleteUserAsync(userId))
                .ThrowsAsync(new Exception("Delete failed"));

            // Act
            // Appelle la méthode DeleteUser du contrôleur pour tester le comportement en cas d'exception.
            var result = await _controller.DeleteUser(userId);

            // Assert
            // Vérifie que le résultat est un ObjectResult, ce qui signifie que le serveur a rencontré une erreur interne.
            var actionResult = Assert.IsType<ObjectResult>(result);

            // Vérifie que le code de statut HTTP est 500 (Internal Server Error), indiquant une erreur interne.
            Assert.Equal((int)HttpStatusCode.InternalServerError, actionResult.StatusCode);

            // Vérifie que le message d'erreur est "Internal server error".
            Assert.Equal(expectedMessage, actionResult.Value);
        }
    }
}
