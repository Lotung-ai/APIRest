using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            _bidRepositoryMock = new Mock<IBidRepository>();
            _loggerMock = new Mock<ILogger<BidListController>>();
            _controller = new BidListController(_bidRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour CreateBid - Cas de succès
        [Fact]
        public async Task CreateBid_ShouldReturnCreatedAtActionResult_WhenBidIsValid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            _bidRepositoryMock.Setup(r => r.CreateBidAsync(bid)).ReturnsAsync(bid);

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetBidById", actionResult.ActionName);
            Assert.Equal(bid.BidListId, actionResult.RouteValues["id"]);
            Assert.Equal(bid, actionResult.Value);
        }

        // Test pour CreateBid - Cas de l'objet null
        [Fact]
        public async Task CreateBid_ShouldReturnBadRequest_WhenBidIsNull()
        {
            // Act
            var result = await _controller.CreateBid(null);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Bid object is null", actionResult.Value);
        }

        // Test pour CreateBid - Cas de validation invalide
        [Fact]
        public async Task CreateBid_ShouldReturnBadRequest_WhenBidIsInvalid()
        {
            // Arrange
            var bid = new BidList();  // Crée un BidList avec des données invalides
            _controller.ModelState.AddModelError("Account", "Account is required.");

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);

            // Assert the ModelState errors
            Assert.True(modelState.ContainsKey("Account"));
            var errors = modelState["Account"] as string[];
            Assert.Contains("Account is required.", errors);
        }

        // Test pour UpdateBid - Cas de succès
        [Fact]
        public async Task UpdateBid_ShouldReturnNoContent_WhenBidIsValid()
        {
            // Arrange
            var originalBid = new BidList
            {
                BidListId = 1,
                Account = "string",
                BidType = "Type1",
                BidQuantity = 0,
                AskQuantity = 0,
                Bid = 0,
                Ask = 0,
                Benchmark = "string",
                BidListDate = DateTime.Now,
                Commentary = "string",
                BidSecurity = "string",
                BidStatus = "string",
                Trader = "string",
                Book = "string",
                CreationName = "string",
                CreationDate = DateTime.Now,
                RevisionName = "string",
                RevisionDate = DateTime.Now,
                DealName = "string",
                DealType = "string",
                SourceListId = "string",
                Side = "string"
            };
            var updatedBid = new BidList
            {
                BidListId = 1,
                Account = "string2",
                BidType = "Type2",
                BidQuantity = 20,
                AskQuantity = 20,
                Bid = 20,
                Ask = 20,
                Benchmark = "string2",
                BidListDate = DateTime.Now,
                Commentary = "string2",
                BidSecurity = "string2",
                BidStatus = "string2",
                Trader = "string2",
                Book = "string2",
                CreationName = "string2",
                CreationDate = DateTime.Now,
                RevisionName = "string2",
                RevisionDate = DateTime.Now,
                DealName = "string2",
                DealType = "string2",
                SourceListId = "string2",
                Side = "string2"
            };

            // Simule que l'entité existe
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(originalBid);
            // Simule la mise à jour réussie
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(updatedBid)).ReturnsAsync(updatedBid);

            // Act - Exécute la méthode à tester
            var result = await _controller.UpdateBid(1, updatedBid);

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedBid = Assert.IsType<BidList>(actionResult.Value);  // Vérifie que la valeur retournée est du type Bid
            Assert.Equal(updatedBid.BidListId, returnedBid.BidListId);  // Vérifie que l'ID du bidlist est correct
            Assert.Equal(updatedBid.Account, returnedBid.Account);  // Vérifie que le Account est correct
            Assert.Equal(updatedBid.BidType, returnedBid.BidType);  // Vérifie que le BidType est correct
        }

        // Test pour UpdateBid - Cas de validation invalide
        [Fact]
        public async Task UpdateBid_ShouldReturnBadRequest_WhenBidIsInvalid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1 };
            _controller.ModelState.AddModelError("Account", "Account is required.");

            // Act
            var result = await _controller.UpdateBid(bid.BidListId, bid);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);

            // Assert the ModelState errors
            Assert.True(modelState.ContainsKey("Account"));
            var errors = modelState["Account"] as string[];
            Assert.Contains("Account is required.", errors);
        }

        // Test pour DeleteBid - Cas de succès
        [Fact]
        public async Task DeleteBid_ShouldReturnNoContent_WhenBidExists()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteBid - Cas de l'objet non trouvé
        [Fact]
        public async Task DeleteBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour GetBidById - Cas de succès
        [Fact]
        public async Task GetBidById_ShouldReturnOkResult_WhenBidExists()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(bid);

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(bid, actionResult.Value);
        }

        // Test pour GetBidById - Cas de l'objet non trouvé
        [Fact]
        public async Task GetBidById_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
