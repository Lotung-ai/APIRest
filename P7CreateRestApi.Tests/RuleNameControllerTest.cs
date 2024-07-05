using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace P7CreateRestApi.Tests
{
    public class RuleNameControllerTests
    {
        private readonly Mock<IRuleRepository> _ruleNameRepositoryMock;
        private readonly Mock<ILogger<RuleNameController>> _loggerMock;
        private readonly RuleNameController _controller;

        public RuleNameControllerTests()
        {
            // Initialisation des mocks et du contrôleur
            _ruleNameRepositoryMock = new Mock<IRuleRepository>();
            _loggerMock = new Mock<ILogger<RuleNameController>>();
            _controller = new RuleNameController(_ruleNameRepositoryMock.Object, _loggerMock.Object);
        }

        private void SetModelStateValidationError()
        {
            // Ajoute une erreur de validation au ModelState pour simuler une erreur de validation dans les tests
            _controller.ModelState.AddModelError("Name", "Name is required.");
        }

        // Test pour CreateRuleName - Cas de succès
        [Fact]
        public async Task CreateRuleName_ShouldReturnCreated_WhenRuleNameIsValid()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "ValidName", Description = "Valid Description", Json = "{}", Template = "Template", SqlStr = "SELECT * FROM Table", SqlPart = "WHERE Condition" };
            _ruleNameRepositoryMock.Setup(r => r.CreateRuleNameAsync(ruleName)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            // Vérifie que l'action retournée est de type CreatedAtActionResult (réponse 201 Created)
            Assert.Equal(nameof(_controller.GetRuleNameById), actionResult.ActionName);
            // Vérifie que l'action visée par le CreatedAtActionResult est GetRuleNameById
            Assert.Equal(ruleName.Id, ((RuleName)actionResult.Value).Id);
            // Vérifie que l'objet RuleName retourné a le même ID que celui passé à la méthode
        }

        // Test pour CreateRuleName - Cas où RuleName est null
        [Fact]
        public async Task CreateRuleName_ShouldReturnBadRequest_WhenRuleNameIsNull()
        {
            // Arrange
            // Crée un RuleName nul pour tester le comportement de la méthode lorsqu'un paramètre invalide est passé.
            RuleName ruleName = null;

            // Act
            // Appelle la méthode CreateRuleName avec le RuleName nul.
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            // Vérifie que la réponse est de type BadRequestObjectResult.
            // Le contrôleur devrait renvoyer un BadRequestObjectResult pour une requête invalide.
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Vérifie que le code de statut de la réponse est 400.
            // 400 Bad Request est le code de statut approprié pour une erreur de validation.
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);

            // Vérifie que le message d'erreur de BadRequestObjectResult est le message attendu.
            // Ce message devrait correspondre exactement à celui que vous avez défini dans le contrôleur.
            Assert.Equal("RuleName cannot be null.", badRequestResult.Value);

            // Vérifie que la valeur de BadRequestObjectResult est bien une chaîne de caractères.
            // Assure que le type de la valeur est correct pour éviter des erreurs de type inattendues.
            Assert.True(badRequestResult.Value is string);
        }

        // Test pour CreateRuleName - Cas où le ModelState est invalide
        [Fact]
        public async Task CreateRuleName_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Description = "Valid Description", Json = "{}", Template = "Template", SqlStr = "SELECT * FROM Table", SqlPart = "WHERE Condition" };
            SetModelStateValidationError();

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            // Vérifie que la réponse est de type BadRequestObjectResult (réponse 400 Bad Request avec des détails)
            Assert.True(_controller.ModelState.ContainsKey("Name"));
            // Vérifie que ModelState contient une erreur pour la clé "Name"
        }

        // Test pour CreateRuleName - Cas d'exception
        [Fact]
        public async Task CreateRuleName_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            // Crée un objet RuleName valide à utiliser pour le test.
            var ruleName = new RuleName
            {
                Id = 1,
                Name = "ValidName",
                Description = "Valid Description",
                Json = "{}",
                Template = "Template",
                SqlStr = "SELECT * FROM Table",
                SqlPart = "WHERE Condition"
            };

            // Configure le mock du repository pour lancer une exception lorsqu'on appelle CreateRuleNameAsync.
            _ruleNameRepositoryMock.Setup(r => r.CreateRuleNameAsync(ruleName))
                                  .ThrowsAsync(new Exception("Test Exception"));

            // Act
            // Appelle la méthode CreateRuleName du contrôleur en passant l'objet RuleName.
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            // Vérifie que le résultat est de type ObjectResult.
            // Cela garantit que le contrôleur renvoie une réponse avec un code de statut 500 pour une erreur interne.
            var statusCodeResult = Assert.IsType<ObjectResult>(result);

            // Vérifie que le code de statut HTTP est 500, ce qui indique une erreur interne du serveur.
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);

            // Vérifie que la valeur de la réponse est le message d'erreur attendu.
            // Le message d'erreur retourné par le contrôleur est "Internal server error." (avec un point à la fin).
            Assert.Equal("An error occurred while retrieving all RuleNames", statusCodeResult.Value);
        }


        // Test pour GetRuleNameById - Cas où RuleName est trouvé et retourné avec succès
        [Fact]
        public async Task GetRuleNameById_ShouldReturnOk_WhenRuleNameExists()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "ValidName", Description = "Valid Description", Json = "{}", Template = "Template", SqlStr = "SELECT * FROM Table", SqlPart = "WHERE Condition" };
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            // Vérifie que la réponse est de type OkObjectResult (réponse 200 OK avec des données)
            var returnedRuleName = Assert.IsType<RuleName>(actionResult.Value);
            // Vérifie que l'objet retourné est de type RuleName
            Assert.Equal(ruleName.Id, returnedRuleName.Id);
            // Vérifie que l'ID du RuleName retourné est correct
            Assert.Equal(ruleName.Name, returnedRuleName.Name);
            // Vérifie que le Name du RuleName retourné est correct
            Assert.Equal(ruleName.Description, returnedRuleName.Description);
            // Vérifie que la Description du RuleName retourné est correcte
            Assert.Equal(ruleName.Json, returnedRuleName.Json);
            // Vérifie que le Json du RuleName retourné est correct
            Assert.Equal(ruleName.Template, returnedRuleName.Template);
            // Vérifie que le Template du RuleName retourné est correct
            Assert.Equal(ruleName.SqlStr, returnedRuleName.SqlStr);
            // Vérifie que le SqlStr du RuleName retourné est correct
            Assert.Equal(ruleName.SqlPart, returnedRuleName.SqlPart);
            // Vérifie que le SqlPart du RuleName retourné est correct
        }

        // Test pour GetRuleNameById - Cas où aucune entité RuleName n'est trouvée
        [Fact]
        public async Task GetRuleNameById_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            // Vérifie que la réponse est de type NotFoundObjectResult (réponse 404 Not Found avec un message d'erreur)
            Assert.Equal(404, notFoundResult.StatusCode);
            // Vérifie que le code de statut est 404 Not Found

            Assert.Equal("No RuleName found with ID 1.", notFoundResult.Value);
            // Vérifie que le message d'erreur est correct
        }


        // Test pour GetRuleNameById - Cas où une exception est lancée
        [Fact]
        public async Task GetRuleNameById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            // Vérifie que la réponse est de type ObjectResult (réponse 500 Internal Server Error avec des détails)
            Assert.Equal(500, actionResult.StatusCode);
            // Vérifie que le code de statut est 500

            // Vérifie que le message d'erreur est "Internal server error." (avec le point à la fin)
            Assert.Equal("An error occurred while retrieving all RuleNames", actionResult.Value);
        }

        // Test pour GetAllRuleNames - Cas où des entités RuleName sont trouvées et retournées avec succès
        [Fact]
        public async Task GetAllRuleNames_ShouldReturnOk_WhenRuleNamesExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curves = new List<RuleName>
            {
             new RuleName { Id = 1, Name = "OldName", Description = "Old Description", Json = "{}", Template = "OldTemplate", SqlStr = "SELECT * FROM OldTable", SqlPart = "WHERE OldCondition" },
            new RuleName { Id = 1, Name = "NewName", Description = "New Description", Json = "{}", Template = "NewTemplate", SqlStr = "SELECT * FROM NewTable", SqlPart = "WHERE NewCondition" }
             };

            _ruleNameRepositoryMock.Setup(r => r.GetAllRuleNamesAsync()).ReturnsAsync(curves);  // Configure le mock pour retourner la liste de courbes

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllRuleNames();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedRuleNames = Assert.IsType<List<RuleName>>(actionResult.Value);  // Vérifie que la valeur retournée est du type List<RuleName>
            Assert.Equal(curves.Count, returnedRuleNames.Count);  // Vérifie que le nombre de courbes retournées est correct
        }

        // Test pour GetAllRuleNames - Cas où aucune entité RuleName n'est trouvée
        [Fact]
        public async Task GetAllRuleNames_ShouldReturnNotFound_WhenNoRuleNamesExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _ruleNameRepositoryMock.Setup(r => r.GetAllRuleNamesAsync()).ReturnsAsync(new List<RuleName>());  // Configure le mock pour retourner une liste vide

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllRuleNames();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour GetAllRuleNames - Cas où une exception est lancée
        [Fact]
        public async Task GetAllRuleNames_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _ruleNameRepositoryMock.Setup(r => r.GetAllRuleNamesAsync()).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllRuleNames();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("An error occurred while retrieving all RuleNames", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }
        // Test pour UpdateRuleName - La mise à jour réussit
        [Fact]
        public async Task UpdateRuleName_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var originalRuleName = new RuleName { Id = 1, Name = "OldName", Description = "Old Description", Json = "{}", Template = "OldTemplate", SqlStr = "SELECT * FROM OldTable", SqlPart = "WHERE OldCondition" };
            var updatedRuleName = new RuleName { Id = 1, Name = "NewName", Description = "New Description", Json = "{}", Template = "NewTemplate", SqlStr = "SELECT * FROM NewTable", SqlPart = "WHERE NewCondition" };

            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync(originalRuleName);
            _ruleNameRepositoryMock.Setup(r => r.UpdateRuleNameAsync(updatedRuleName)).ReturnsAsync(updatedRuleName);

            // Act
            var result = await _controller.UpdateRuleName(1, updatedRuleName);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            // Vérifie que la réponse est de type OkObjectResult (réponse 200 OK avec des données)
            Assert.Equal(updatedRuleName, actionResult.Value);
            // Vérifie que le RuleName retourné est bien l'objet mis à jour
        }

        // Test pour UpdateRuleName - ID dans l'URL ne correspond pas à l'ID de l'objet
        [Fact]
        public async Task UpdateRuleName_ShouldReturnBadRequest_WhenIdMismatchOccurs()
        {
            // Arrange
            var ruleName = new RuleName { Id = 2, Name = "NewName", Description = "New Description", Json = "{}", Template = "NewTemplate", SqlStr = "SELECT * FROM NewTable", SqlPart = "WHERE NewCondition" };

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            // Vérifie que la réponse est de type BadRequestObjectResult (réponse 400 Bad Request avec des détails)

            Assert.True(badRequestResult.Value is SerializableError);
            // Vérifie que la valeur de la réponse est de type SerializableError
            var errors = badRequestResult.Value as SerializableError;

            Assert.True(errors.ContainsKey("IdMismatch"));
            // Vérifie que l'erreur "IdMismatch" est présente dans les détails de l'erreur
            var errorMessages = errors["IdMismatch"] as string[];
            Assert.Contains("The ID in the URL does not match the ID in the RuleName object.", errorMessages);
            // Vérifie que le message d'erreur contient la description attendue pour le problème de correspondance d'ID
        }


        // Test pour UpdateRuleName - L'élément à mettre à jour n'existe pas
        [Fact]
        public async Task UpdateRuleName_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "NewName", Description = "New Description", Json = "{}", Template = "NewTemplate", SqlStr = "SELECT * FROM NewTable", SqlPart = "WHERE NewCondition" };
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            // Vérifie que la réponse est de type NotFoundObjectResult (réponse 404 Not Found avec des détails)
            Assert.Equal(404, notFoundResult.StatusCode);
            // Vérifie que le code de statut est 404
            Assert.Equal($"No RuleName found with ID 1.", notFoundResult.Value);
            // Vérifie que le message d'erreur est le message attendu
        }


        // Test pour UpdateRuleName - Une exception est levée
        [Fact]
        public async Task UpdateRuleName_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "NewName", Description = "New Description", Json = "{}", Template = "NewTemplate", SqlStr = "SELECT * FROM NewTable", SqlPart = "WHERE NewCondition" };
            _ruleNameRepositoryMock.Setup(r => r.UpdateRuleNameAsync(ruleName)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            // Vérifie que la réponse est de type ObjectResult (réponse 500 Internal Server Error avec des détails)
            Assert.Equal(500, actionResult.StatusCode);
            // Vérifie que le code de statut est 500

            // Assurez-vous que l'objet retourné contient le message d'erreur approprié
            var errorMessage = Assert.IsType<string>(actionResult.Value);
            Assert.Equal("An error occurred while retrieving all RuleNames", errorMessage.Trim());
            // Vérifie que le message d'erreur est "Internal server error" en supprimant les espaces superflus
        }

        // Test pour DeleteRuleName - Suppression réussie
        [Fact]
        public async Task DeleteRuleName_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            int ruleNameId = 1;
            _ruleNameRepositoryMock.Setup(r => r.DeleteRuleNameAsync(ruleNameId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRuleName(ruleNameId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            // Vérifie que la réponse est de type NoContentResult (réponse 204 No Content)
        }

        // Test pour DeleteRuleName - RuleName non trouvé
        [Fact]
        public async Task DeleteRuleName_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            int ruleNameId = 1;
            _ruleNameRepositoryMock.Setup(r => r.DeleteRuleNameAsync(ruleNameId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRuleName(ruleNameId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            // Vérifie que la réponse est de type NotFoundObjectResult (réponse 404 Not Found avec un message d'erreur)
            Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
            // Vérifie que le code de statut est 404 Not Found

            // Vérifie que le message d'erreur correspond à ce qui est attendu
            Assert.Equal($"No RuleName found with ID {ruleNameId}.", notFoundResult.Value);
        }

        // Test pour DeleteRuleName - Exception levée
        [Fact]
        public async Task DeleteRuleName_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int ruleNameId = 1;

            // Configure le mock du repository pour lancer une exception lorsqu'on appelle DeleteRuleNameAsync.
            _ruleNameRepositoryMock.Setup(r => r.DeleteRuleNameAsync(ruleNameId))
                                  .ThrowsAsync(new Exception("Delete failed"));

            // Act
            // Appelle la méthode DeleteRuleName du contrôleur avec l'ID du RuleName.
            var result = await _controller.DeleteRuleName(ruleNameId);

            // Assert
            // Vérifie que le résultat est de type ObjectResult.
            // Cela garantit que le contrôleur renvoie une réponse avec un code de statut 500 pour une erreur interne.
            var actionResult = Assert.IsType<ObjectResult>(result);

            // Vérifie que le code de statut HTTP est 500, ce qui indique une erreur interne du serveur.
            Assert.Equal((int)HttpStatusCode.InternalServerError, actionResult.StatusCode);

            // Vérifie que la valeur de la réponse est le message d'erreur attendu.
            // Le message d'erreur retourné par le contrôleur est "Internal server error." (avec un point à la fin).
            Assert.Equal("An error occurred while retrieving all RuleNames", actionResult.Value);
        }

    }
}
