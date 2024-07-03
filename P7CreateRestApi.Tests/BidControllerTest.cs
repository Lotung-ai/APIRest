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
            // Initialisation des mocks et du contr�leur
            _bidRepositoryMock = new Mock<IBidRepository>();
            _loggerMock = new Mock<ILogger<BidListController>>();
            _controller = new BidListController(_bidRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour CreateBid - Cas de succ�s
        [Fact]
        public async Task CreateBid_ShouldReturnCreated_WhenBidIsValid()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.CreateBidAsync(bid)).ReturnsAsync(bid);  // Configure le mock pour retourner le bid

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.CreateBid(bid);

            // Assert - V�rifie que le r�sultat est correct
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);  // V�rifie que le r�sultat est du type CreatedAtActionResult (HTTP 201 Created)
            Assert.Equal(nameof(_controller.GetBidById), actionResult.ActionName);  // V�rifie que le nom de l'action est correct
            Assert.Equal(bid.BidListId, ((BidList)actionResult.Value).BidListId);  // V�rifie que l'ID du bid est correct
        }

        // Test pour CreateBid - Cas o� Bid est null
        [Fact]
        public async Task CreateBid_ShouldReturnBadRequest_WhenBidIsNull()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            BidList bid = null;

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.CreateBid(bid);

            // Assert - V�rifie que le r�sultat est correct
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // V�rifie que le r�sultat est du type BadRequestObjectResult (HTTP 400 Bad Request)
            Assert.Equal("Bid object is null", badRequestResult.Value);  // V�rifie que le message d'erreur est correct
        }

        // Test pour CreateBid - Cas d'exception
        [Fact]
        public async Task CreateBid_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.CreateBidAsync(bid)).ThrowsAsync(new Exception("Test Exception"));  // Configure le mock pour lancer une exception

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.CreateBid(bid);

            // Assert - V�rifie que le r�sultat est correct
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // V�rifie que le r�sultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, statusCodeResult.StatusCode);  // V�rifie que le code de statut est 500
            Assert.Equal("Internal server error", statusCodeResult.Value);  // V�rifie que le message d'erreur est correct
        }

        // Test pour GetBidById - Cas o� Bid est trouv� et retourn� avec succ�s
        [Fact]
        public async Task GetBidById_ShouldReturnOk_WhenBidExists()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(bid);  // Configure le mock pour retourner le bid

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.GetBidById(1);

            // Assert - V�rifie que le r�sultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // V�rifie que le r�sultat est du type OkObjectResult (HTTP 200 OK)
            var returnedBid = Assert.IsType<BidList>(actionResult.Value);  // V�rifie que la valeur retourn�e est du type BidList
            Assert.Equal(bid.BidListId, returnedBid.BidListId);  // V�rifie que l'ID du bid est correct
            Assert.Equal(bid.Account, returnedBid.Account);  // V�rifie que le compte du bid est correct
            Assert.Equal(bid.Bid, returnedBid.Bid);  // V�rifie que la valeur du bid est correcte
        }

        // Test pour GetBidById - Cas o� aucune entit� Bid n'est trouv�e
        [Fact]
        public async Task GetBidById_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);  // Configure le mock pour retourner null

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.GetBidById(1);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<NotFoundResult>(result);  // V�rifie que le r�sultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour GetBidById - Cas o� une exception est lanc�e
        [Fact]
        public async Task GetBidById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.GetBidById(1);

            // Assert - V�rifie que le r�sultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // V�rifie que le r�sultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // V�rifie que le code de statut est 500
            Assert.Equal("Internal server error", actionResult.Value);  // V�rifie que le message d'erreur est correct
        }

        // Test pour UpdateBid - La mise � jour r�ussit
        [Fact]
        public async Task UpdateBid_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            var originalBid = new BidList { BidListId = 1, Account = "Old Account", Bid = 50.0 };
            var updatedBid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };

            // Simule que l'entit� existe
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(originalBid);

            // Simule la mise � jour r�ussie
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(updatedBid));

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.UpdateBid(1, updatedBid);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<NoContentResult>(result);  // V�rifie que le r�sultat est NoContentResult (HTTP 204 No Content)

            // V�rifie que UpdateBidAsync a �t� appel� avec l'objet `updatedBid`
            _bidRepositoryMock.Verify(r => r.UpdateBidAsync(updatedBid), Times.Once);

            // V�rifie que les propri�t�s de l'objet mis � jour sont correctes
            Assert.Equal("Test Account", updatedBid.Account);
            Assert.Equal(100.0, updatedBid.Bid);
        }

        // Test pour UpdateBid - Bid est nul ou ID ne correspond pas
        [Fact]
        public async Task UpdateBid_ShouldReturnBadRequest_WhenBidIsNullOrIdMismatch()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            BidList bid = null;

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.UpdateBid(1, bid);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<BadRequestObjectResult>(result);  // V�rifie que le r�sultat est du type BadRequestObjectResult (HTTP 400 Bad Request)

            // Arrange with ID mismatch - Pr�pare un bid avec un ID diff�rent
            bid = new BidList { BidListId = 2, Account = "Test Account", Bid = 100.0 };

            // Act - Ex�cute la m�thode � tester
            result = await _controller.UpdateBid(1, bid);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<BadRequestObjectResult>(result);  // V�rifie que le r�sultat est du type BadRequestObjectResult (HTTP 400 Bad Request)
        }

        // Test pour UpdateBid - L'�l�ment � mettre � jour n'existe pas
        [Fact]
        public async Task UpdateBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.UpdateBid(1, bid);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<NotFoundResult>(result);  // V�rifie que le r�sultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour UpdateBid - Une exception est lev�e
        [Fact]
        public async Task UpdateBid_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            var bid = new BidList { BidListId = 1, Account = "Test Account", Bid = 100.0 };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(bid);
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(bid)).ThrowsAsync(new Exception("Database error"));

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.UpdateBid(1, bid);

            // Assert - V�rifie que le r�sultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // V�rifie que le r�sultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // V�rifie que le code de statut est 500
            Assert.Equal("Internal server error", actionResult.Value);  // V�rifie que le message d'erreur est correct
        }

        // Test pour DeleteBid - Suppression r�ussie
        [Fact]
        public async Task DeleteBid_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            int bidId = 1;
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(bidId)).ReturnsAsync(true);

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.DeleteBid(bidId);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<NoContentResult>(result);  // V�rifie que le r�sultat est NoContentResult (HTTP 204 No Content)
        }

        // Test pour DeleteBid - Bid non trouv�
        [Fact]
        public async Task DeleteBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            int bidId = 1;
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(bidId)).ReturnsAsync(false);

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.DeleteBid(bidId);

            // Assert - V�rifie que le r�sultat est correct
            Assert.IsType<NotFoundResult>(result);  // V�rifie que le r�sultat est NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour DeleteBid - Exception lev�e
        [Fact]
        public async Task DeleteBid_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Pr�pare les objets n�cessaires pour le test
            int bidId = 1;
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(bidId)).ThrowsAsync(new Exception("Delete failed"));

            // Act - Ex�cute la m�thode � tester
            var result = await _controller.DeleteBid(bidId);

            // Assert - V�rifie que le r�sultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // V�rifie que le r�sultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // V�rifie que le code de statut est 500
            Assert.Equal("Internal server error", actionResult.Value);  // V�rifie que le message d'erreur est correct
        }
    }
}
