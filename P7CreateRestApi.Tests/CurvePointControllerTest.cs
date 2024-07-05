using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace P7CreateRestApi.Tests
{
    public class CurveControllerTests
    {
        private readonly Mock<ICurvePointRepository> _curveRepositoryMock;
        private readonly Mock<ILogger<CurveController>> _loggerMock;
        private readonly CurveController _controller;

        public CurveControllerTests()
        {
            // Initialisation des mocks et du contrôleur
            _curveRepositoryMock = new Mock<ICurvePointRepository>();
            _loggerMock = new Mock<ILogger<CurveController>>();
            _controller = new CurveController(_curveRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour CreateCurvePoint - Cas de succès
        [Fact]
        public async Task CreateCurvePoint_ShouldReturnCreated_WhenCurvePointIsValid()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curve = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.UtcNow,
                Term = 1.0,
                CurvePointValue = 100.0,
                CreationDate = DateTime.UtcNow
            };

            _curveRepositoryMock.Setup(r => r.CreateCurvePointAsync(curve)).ReturnsAsync(curve);  // Configure le mock pour retourner le curve

            // Act - Exécute la méthode à tester
            var result = await _controller.CreateCurvePoint(curve);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);  // Vérifie que le résultat est du type CreatedAtActionResult (HTTP 201 Created)
            Assert.Equal(nameof(_controller.GetCurvePointById), actionResult.ActionName);  // Vérifie que le nom de l'action est correct
            Assert.Equal(curve.Id, ((CurvePoint)actionResult.Value).Id);  // Vérifie que l'ID du curve est correct
        }

        // Test pour CreateCurvePoint - Cas où CurvePoint est null
        [Fact]
        public async Task CreateCurvePoint_ShouldReturnBadRequest_WhenCurvePointIsNull()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            CurvePoint curve = null;

            // Act - Exécute la méthode à tester
            var result = await _controller.CreateCurvePoint(curve);

            // Assert - Vérifie que le résultat est correct
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est du type BadRequestObjectResult (HTTP 400 Bad Request)
            Assert.Equal("CurvePoint cannot be null.", badRequestResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour CreateCurvePoint - Cas d'exception
        [Fact]
        public async Task CreateCurvePoint_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curve = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.UtcNow,
                Term = 1.0,
                CurvePointValue = 100.0,
                CreationDate = DateTime.UtcNow
            };

            _curveRepositoryMock.Setup(r => r.CreateCurvePointAsync(curve)).ThrowsAsync(new Exception("Test Exception"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.CreateCurvePoint(curve);

            // Assert - Vérifie que le résultat est correct
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, statusCodeResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("An error occurred while creating the CurvePoint.", statusCodeResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour GetCurvePointById - Cas où CurvePoint est trouvé et retourné avec succès
        [Fact]
        public async Task GetCurvePointById_ShouldReturnOk_WhenCurvePointExists()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curve = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.UtcNow,
                Term = 1.0,
                CurvePointValue = 100.0,
                CreationDate = DateTime.UtcNow
            };

            _curveRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync(curve);  // Configure le mock pour retourner le curve

            // Act - Exécute la méthode à tester
            var result = await _controller.GetCurvePointById(1);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedCurvePoint = Assert.IsType<CurvePoint>(actionResult.Value);  // Vérifie que la valeur retournée est du type CurvePoint
            Assert.Equal(curve.Id, returnedCurvePoint.Id);  // Vérifie que l'ID du curve est correct
            Assert.Equal(curve.CurveId, returnedCurvePoint.CurveId);  // Vérifie que l'ID de la courbe est correct
            Assert.Equal(curve.AsOfDate, returnedCurvePoint.AsOfDate);  // Vérifie que la date de la courbe est correcte
            Assert.Equal(curve.Term, returnedCurvePoint.Term);  // Vérifie que le terme de la courbe est correct
            Assert.Equal(curve.CurvePointValue, returnedCurvePoint.CurvePointValue);  // Vérifie que la valeur du point de courbe est correcte
        }

        // Test pour GetCurvePointById - Cas où aucune entité CurvePoint n'est trouvée
        [Fact]
        public async Task GetCurvePointById_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _curveRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync((CurvePoint)null);  // Configure le mock pour retourner null

            // Act - Exécute la méthode à tester
            var result = await _controller.GetCurvePointById(1);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour GetCurvePointById - Cas où une exception est lancée
        [Fact]
        public async Task GetCurvePointById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _curveRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.GetCurvePointById(1);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("An error occurred while retrieving the CurvePoint.", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour GetAllCurvePoints - Cas où des entités CurvePoint sont trouvées et retournées avec succès
        [Fact]
        public async Task GetAllCurvePoints_ShouldReturnOk_WhenCurvePointsExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curves = new List<CurvePoint>
            {
                new CurvePoint
                {
                    Id = 1,
                    CurveId = 1,
                    AsOfDate = DateTime.UtcNow,
                    Term = 1.0,
                    CurvePointValue = 100.0,
                    CreationDate = DateTime.UtcNow
                },
                new CurvePoint
                {
                    Id = 2,
                    CurveId = 2,
                    AsOfDate = DateTime.UtcNow,
                    Term = 2.0,
                    CurvePointValue = 200.0,
                    CreationDate = DateTime.UtcNow
                }
            };

            _curveRepositoryMock.Setup(r => r.GetAllCurvePointsAsync()).ReturnsAsync(curves);  // Configure le mock pour retourner la liste de courbes

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllCurvePoints();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedCurvePoints = Assert.IsType<List<CurvePoint>>(actionResult.Value);  // Vérifie que la valeur retournée est du type List<CurvePoint>
            Assert.Equal(curves.Count, returnedCurvePoints.Count);  // Vérifie que le nombre de courbes retournées est correct
        }

        // Test pour GetAllCurvePoints - Cas où aucune entité CurvePoint n'est trouvée
        [Fact]
        public async Task GetAllCurvePoints_ShouldReturnNotFound_WhenNoCurvePointsExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _curveRepositoryMock.Setup(r => r.GetAllCurvePointsAsync()).ReturnsAsync(new List<CurvePoint>());  // Configure le mock pour retourner une liste vide

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllCurvePoints();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour GetAllCurvePoints - Cas où une exception est lancée
        [Fact]
        public async Task GetAllCurvePoints_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _curveRepositoryMock.Setup(r => r.GetAllCurvePointsAsync()).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllCurvePoints();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("An error occurred while retrieving all CurvePoints.", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour UpdateCurvePoint - La mise à jour réussit
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var originalCurvePoint = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.Now,
                Term = 1.0,
                CurvePointValue = 100.0,
                CreationDate = DateTime.Now
            };

            var updatedCurvePoint = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.Now,
                Term = 2.0,
                CurvePointValue = 200.0,
                CreationDate = DateTime.Now
            };

            // Simule que l'entité existe
            _curveRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync(originalCurvePoint);
            // Simule la mise à jour réussie
            _curveRepositoryMock.Setup(r => r.UpdateCurvePointAsync(updatedCurvePoint)).ReturnsAsync(updatedCurvePoint);

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateCurvePoint(1, updatedCurvePoint);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedCurvePoint = Assert.IsType<CurvePoint>(actionResult.Value);  // Vérifie que la valeur retournée est du type CurvePoint
            Assert.Equal(updatedCurvePoint.Id, returnedCurvePoint.Id);  // Vérifie que l'ID du curve est correct
            Assert.Equal(updatedCurvePoint.CurveId, returnedCurvePoint.CurveId);  // Vérifie que le CurveId est correct
            Assert.Equal(updatedCurvePoint.AsOfDate, returnedCurvePoint.AsOfDate);  // Vérifie que le AsOfDate est correct
            Assert.Equal(updatedCurvePoint.Term, returnedCurvePoint.Term);  // Vérifie que le Term est correct
            Assert.Equal(updatedCurvePoint.CurvePointValue, returnedCurvePoint.CurvePointValue);  // Vérifie que le CurvePointValue est correct
            Assert.Equal(updatedCurvePoint.CreationDate, returnedCurvePoint.CreationDate);  // Vérifie que le CreationDate est correct

        }

        // Test pour UpdateCurvePoint - CurvePoint est nul
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnBadRequest_WhenCurvePointIsNull()
        {
            // Arrange
            CurvePoint curve = null;

            // Act
            var result = await _controller.UpdateCurvePoint(1, curve);

            // Assert
            // Vérifie que le résultat est de type BadRequestObjectResult.
            // La méthode UpdateCurvePoint doit retourner un BadRequest avec un message d'erreur si l'objet CurvePoint est null.
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Vérifie que le code de statut HTTP est 400.
            // C'est le code de réponse standard pour indiquer une demande incorrecte due à des données invalides (dans ce cas, CurvePoint est null).
            Assert.Equal(400, badRequestResult.StatusCode);

            // Vérifie que le message de la réponse est "CurvePoint object is null".
            // Ce message est celui qui est renvoyé par la méthode UpdateCurvePoint pour indiquer que l'objet CurvePoint ne peut pas être null.
            Assert.Equal("CurvePoint object is null", badRequestResult.Value);
        }

        // Test pour UpdateCurvePoint - ID ne correspond pas
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnBadRequest_WhenIdDoesNotMatch()
        {
            // Arrange
            var curve = new CurvePoint
            {
                Id = 2,
                CurveId = 2,
                AsOfDate = DateTime.Now,
                Term = 2.0,
                CurvePointValue = 200.0,
                CreationDate = DateTime.Now
            };
            // Act
            var result = await _controller.UpdateCurvePoint(1, curve);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est du type BadRequestObjectResult (HTTP 400 Bad Request)
            Assert.True(badRequestResult.Value is SerializableError);
            var errors = badRequestResult.Value as SerializableError;

            Assert.True(errors.ContainsKey("IdMismatch"));  // Vérifie que le modèle d'état contient une clé "IdMismatch"
            var errorMessages = errors["IdMismatch"] as string[];
            Assert.Contains("The curvePoint ID in the URL does not match the ID in the curve object.", errorMessages);  // Vérifie le message d'erreur spécifique
        }

        // Test pour UpdateCurvePoint - L'élément à mettre à jour n'existe pas
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curve = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.Now,
                Term = 2.0,
                CurvePointValue = 200.0,
                CreationDate = DateTime.Now
            };
            _curveRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync((CurvePoint)null);

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateCurvePoint(1, curve);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour UpdateCurvePoint - Une exception est levée
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnErrorMessage_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curve = new CurvePoint
            {
                Id = 1,
                CurveId = 1,
                AsOfDate = DateTime.Now,
                Term = 2.0,
                CurvePointValue = 200.0,
                CreationDate = DateTime.Now
            };
            _curveRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync(curve);
            _curveRepositoryMock.Setup(r => r.UpdateCurvePointAsync(curve)).ThrowsAsync(new Exception("Database error"));

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateCurvePoint(1, curve);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("UpdateCurvePoint: An error occurred while updating the curvePoint with ID", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour DeleteCurvePoint - Suppression réussie
        [Fact]
        public async Task DeleteCurvePoint_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            int curveId = 1;
            _curveRepositoryMock.Setup(r => r.DeleteCurvePointAsync(curveId)).ReturnsAsync(true);

            // Act - Exécute la méthode à tester
            var result = await _controller.DeleteCurvePoint(curveId);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NoContentResult>(result);  // Vérifie que le résultat est NoContentResult (HTTP 204 No Content)
        }

        // Test pour DeleteCurvePoint - CurvePoint non trouvé
        [Fact]
        public async Task DeleteCurvePoint_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            int curveId = 1;
            _curveRepositoryMock.Setup(r => r.DeleteCurvePointAsync(curveId)).ReturnsAsync(false);

            // Act - Exécute la méthode à tester
            var result = await _controller.DeleteCurvePoint(curveId);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour DeleteCurvePoint - Exception levée
        [Fact]
        public async Task DeleteCurvePoint_ShouldReturnErrorMessage_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            int curveId = 1;
            _curveRepositoryMock.Setup(r => r.DeleteCurvePointAsync(curveId)).ThrowsAsync(new Exception("Delete failed"));

            // Act - Exécute la méthode à tester
            var result = await _controller.DeleteCurvePoint(curveId);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("An error occurred while deleting the CurvePoint.", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }
    }
}

