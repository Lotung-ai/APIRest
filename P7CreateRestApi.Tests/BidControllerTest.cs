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
    public class BidListControllerTests
    {
        private readonly Mock<IBidRepository> _bidRepositoryMock;
        private readonly Mock<ILogger<BidListController>> _loggerMock;
        private readonly BidListController _controller;

        public BidListControllerTests()
        {
            // Initialisation des mocks et du contrôleur
            _bidRepositoryMock = new Mock<IBidRepository>();
            _loggerMock = new Mock<ILogger<BidListController>>();
            _controller = new BidListController(_bidRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour CreateBid - Cas de succès
        [Fact]
        public async Task CreateBid_ShouldReturnCreated_WhenBidIsValid()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.CreateBidAsync(bid)).ReturnsAsync(bid);  // Configure le mock pour retourner le bid

            // Act - Exécute la méthode à tester
            var result = await _controller.CreateBid(bid);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);  // Vérifie que le résultat est du type CreatedAtActionResult (HTTP 201 Created)
            Assert.Equal(nameof(_controller.GetBidById), actionResult.ActionName);  // Vérifie que le nom de l'action est correct
            Assert.Equal(bid.BidListId, ((BidList)actionResult.Value).BidListId);  // Vérifie que l'ID du bid est correct
        }

        // Test pour CreateBid - Cas où Bid est null
        [Fact]
        public async Task CreateBid_ShouldReturnBadRequest_WhenBidIsNull()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            BidList bid = null;

            // Act - Exécute la méthode à tester
            var result = await _controller.CreateBid(bid);

            // Assert - Vérifie que le résultat est correct
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est du type BadRequestObjectResult (HTTP 400 Bad Request)
            Assert.Equal("Bid object is null", badRequestResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour CreateBid - Cas d'exception
        [Fact]
        public async Task CreateBid_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.CreateBidAsync(bid)).ThrowsAsync(new Exception("Test Exception"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.CreateBid(bid);

            // Assert - Vérifie que le résultat est correct
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, statusCodeResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("Internal server error", statusCodeResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour GetBidById - Cas où Bid est trouvé et retourné avec succès
        [Fact]
        public async Task GetBidById_ShouldReturnOk_WhenBidExists()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(bid);  // Configure le mock pour retourner le bid

            // Act - Exécute la méthode à tester
            var result = await _controller.GetBidById(1);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedBid = Assert.IsType<BidList>(actionResult.Value);  // Vérifie que la valeur retournée est du type BidList
            Assert.Equal(bid.BidListId, returnedBid.BidListId);  // Vérifie que l'ID du bid est correct
            Assert.Equal(bid.Account, returnedBid.Account);  // Vérifie que le compte du bid est correct
            Assert.Equal(bid.Bid, returnedBid.Bid);  // Vérifie que la valeur du bid est correcte
        }

        // Test pour GetBidById - Cas où aucune entité Bid n'est trouvée
        [Fact]
        public async Task GetBidById_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);  // Configure le mock pour retourner null

            // Act - Exécute la méthode à tester
            var result = await _controller.GetBidById(1);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour GetBidById - Cas où une exception est lancée
        [Fact]
        public async Task GetBidById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.GetBidById(1);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("Internal server error", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour UpdateBid - La mise à jour réussit
        [Fact]
        public async Task UpdateBid_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var originalBid = new BidList { BidListId = 1, Account = "Old Account", Bid = 50.0 };
            var updatedBid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };

            // Simule que l'entité existe
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(originalBid);

            // Simule la mise à jour réussie
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(updatedBid));

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateBid(1, updatedBid);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NoContentResult>(result);  // Vérifie que le résultat est NoContentResult (HTTP 204 No Content)

            // Vérifie que UpdateBidAsync a été appelé avec l'objet `updatedBid`
            _bidRepositoryMock.Verify(r => r.UpdateBidAsync(updatedBid), Times.Once);

            // Vérifie que les propriétés de l'objet mis à jour sont correctes
            Assert.Equal("Test Account", updatedBid.Account);
            Assert.Equal(100.0, updatedBid.Bid);
        }

        // Test pour UpdateBid - Bid est nul ou ID ne correspond pas
        [Fact]
        public async Task UpdateBid_ShouldReturnBadRequest_WhenBidIsNullOrIdMismatch()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            BidList bid = null;

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateBid(1, bid);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est du type BadRequestObjectResult (HTTP 400 Bad Request)

            // Arrange with ID mismatch - Prépare un bid avec un ID différent
            bid = new BidList { BidListId = 2, Account = "Test Account", Bid = 100.0 };

            // Act - Exécute la méthode à tester
            result = await _controller.UpdateBid(1, bid);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est du type BadRequestObjectResult (HTTP 400 Bad Request)
        }

        // Test pour UpdateBid - L'élément à mettre à jour n'existe pas
        [Fact]
        public async Task UpdateBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateBid(1, bid);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour UpdateBid - Une exception est levée
        [Fact]
        public async Task UpdateBid_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(bid);
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(bid)).ThrowsAsync(new Exception("Database error"));

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateBid(1, bid);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("Internal server error", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour DeleteBid - Suppression réussie
        [Fact]
        public async Task DeleteBid_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            int bidId = 1;
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(bidId)).ReturnsAsync(true);

            // Act - Exécute la méthode à tester
            var result = await _controller.DeleteBid(bidId);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NoContentResult>(result);  // Vérifie que le résultat est NoContentResult (HTTP 204 No Content)
        }

        // Test pour DeleteBid - Bid non trouvé
        [Fact]
        public async Task DeleteBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            int bidId = 1;
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(bidId)).ReturnsAsync(false);

            // Act - Exécute la méthode à tester
            var result = await _controller.DeleteBid(bidId);

            // Assert - Vérifie que le résultat est correct
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour DeleteBid - Exception levée
        [Fact]
        public async Task DeleteBid_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            int bidId = 1;
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(bidId)).ThrowsAsync(new Exception("Delete failed"));

            // Act - Exécute la méthode à tester
            var result = await _controller.DeleteBid(bidId);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("Internal server error", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }
    }
}
