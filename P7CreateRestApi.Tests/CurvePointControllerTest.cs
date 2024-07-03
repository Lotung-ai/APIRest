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
    }
}
