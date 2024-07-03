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
            var bid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(bid.BidListId)).ReturnsAsync(bid);
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(bid));

            // Act
            var result = await _controller.UpdateBid(bid.BidListId, bid);

            // Assert
            Assert.IsType<NoContentResult>(result);
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
