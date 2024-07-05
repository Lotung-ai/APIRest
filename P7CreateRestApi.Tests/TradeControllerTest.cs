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
    public class TradeControllerTests
    {
        private readonly Mock<ITradeRepository> _tradeRepositoryMock;
        private readonly Mock<ILogger<TradeController>> _loggerMock;
        private readonly TradeController _controller;

        public TradeControllerTests()
        {
            // Initialisation des mocks et du contrôleur
            _tradeRepositoryMock = new Mock<ITradeRepository>();
            _loggerMock = new Mock<ILogger<TradeController>>();
            _controller = new TradeController(_tradeRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour CreateTrade - Cas de succès
        [Fact]
        public async Task CreateTrade_ShouldReturnCreated_WhenTradeIsValid()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 1,
                Account = "Account1",
                AccountType = "TypeA",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 15.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "Security1",
                TradeStatus = "Active",
                Trader = "Trader1",
                Benchmark = "Benchmark1",
                Book = "Book1",
                CreationName = "Creator1",
                CreationDate = DateTime.UtcNow,
                RevisionName = "Revisor1",
                RevisionDate = DateTime.UtcNow,
                DealName = "Deal1",
                DealType = "Type1",
                SourceListId = "List1",
                Side = "Buy"
            };
            _tradeRepositoryMock.Setup(r => r.CreateTradeAsync(trade)).ReturnsAsync(trade);

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);  // Vérifie que le résultat est de type CreatedAtActionResult
            Assert.Equal(nameof(_controller.GetTradeById), actionResult.ActionName);  // Vérifie que l'action redirige vers GetTradeById
            Assert.Equal(trade.TradeId, ((Trade)actionResult.Value).TradeId);  // Vérifie que l'ID de la trade dans la réponse est correct
        }

        // Test pour CreateTrade - Cas où Trade est null
        [Fact]
        public async Task CreateTrade_ShouldReturnBadRequest_WhenTradeIsNull()
        {
            // Arrange
            Trade trade = null;

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est de type BadRequestObjectResult
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);  // Vérifie que le code d'état est 400 (Bad Request)
            Assert.Equal("Trade cannot be null.", badRequestResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour CreateTrade - Cas où le ModelState est invalide
        [Fact]
        public async Task CreateTrade_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 1,
                Account = "Account1",
                AccountType = "TypeA",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 15.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "Security1",
                TradeStatus = "Active",
                Trader = "Trader1",
                Benchmark = "Benchmark1",
                Book = "Book1",
                CreationName = "Creator1",
                CreationDate = DateTime.UtcNow,
                RevisionName = "Revisor1",
                RevisionDate = DateTime.UtcNow,
                DealName = "Deal1",
                DealType = "Type1",
                SourceListId = "List1",
                Side = "Buy"
            };
            _controller.ModelState.AddModelError("Account", "Account is required.");  // Ajoute une erreur de validation au ModelState

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est de type BadRequestObjectResult
            Assert.True(_controller.ModelState.ContainsKey("Account"));  // Vérifie que l'erreur de validation est présente dans le ModelState
            Assert.Equal("Account is required.", _controller.ModelState["Account"].Errors[0].ErrorMessage);  // Vérifie que le message d'erreur est correct
        }

        // Test pour CreateTrade - Cas d'exception
        [Fact]
        public async Task CreateTrade_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 1,
                Account = "Account1",
                AccountType = "TypeA",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 15.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "Security1",
                TradeStatus = "Active",
                Trader = "Trader1",
                Benchmark = "Benchmark1",
                Book = "Book1",
                CreationName = "Creator1",
                CreationDate = DateTime.UtcNow,
                RevisionName = "Revisor1",
                RevisionDate = DateTime.UtcNow,
                DealName = "Deal1",
                DealType = "Type1",
                SourceListId = "List1",
                Side = "Buy"
            };

            _tradeRepositoryMock.Setup(r => r.CreateTradeAsync(trade))
                .ThrowsAsync(new Exception("Test Exception"));  // Configure le mock pour lancer une exception

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est de type ObjectResult
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);  // Vérifie que le code d'état est 500 (Internal Server Error)
            Assert.Equal("An error occurred while retrieving the Trade ID", statusCodeResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour GetTradeById - Cas où Trade est trouvé et retourné avec succès
        [Fact]
        public async Task GetTradeById_ShouldReturnOk_WhenTradeExists()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 1,
                Account = "Account1",
                AccountType = "TypeA",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 15.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "Security1",
                TradeStatus = "Active",
                Trader = "Trader1",
                Benchmark = "Benchmark1",
                Book = "Book1",
                CreationName = "Creator1",
                CreationDate = DateTime.UtcNow,
                RevisionName = "Revisor1",
                RevisionDate = DateTime.UtcNow,
                DealName = "Deal1",
                DealType = "Type1",
                SourceListId = "List1",
                Side = "Buy"
            };
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync(trade);

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est de type OkObjectResult
            var returnedTrade = Assert.IsType<Trade>(actionResult.Value);  // Vérifie que la valeur retournée est de type Trade
            Assert.Equal(trade.TradeId, returnedTrade.TradeId);  // Vérifie que l'ID de la trade dans la réponse est correct
            Assert.Equal(trade.Account, returnedTrade.Account);  // Vérifie que les autres propriétés de la trade sont correctement retournées
            Assert.Equal(trade.AccountType, returnedTrade.AccountType);
            Assert.Equal(trade.BuyQuantity, returnedTrade.BuyQuantity);
            Assert.Equal(trade.SellQuantity, returnedTrade.SellQuantity);
            Assert.Equal(trade.BuyPrice, returnedTrade.BuyPrice);
            Assert.Equal(trade.SellPrice, returnedTrade.SellPrice);
            Assert.Equal(trade.TradeDate, returnedTrade.TradeDate);
            Assert.Equal(trade.TradeSecurity, returnedTrade.TradeSecurity);
            Assert.Equal(trade.TradeStatus, returnedTrade.TradeStatus);
            Assert.Equal(trade.Trader, returnedTrade.Trader);
            Assert.Equal(trade.Benchmark, returnedTrade.Benchmark);
            Assert.Equal(trade.Book, returnedTrade.Book);
            Assert.Equal(trade.CreationName, returnedTrade.CreationName);
            Assert.Equal(trade.CreationDate, returnedTrade.CreationDate);
            Assert.Equal(trade.RevisionName, returnedTrade.RevisionName);
            Assert.Equal(trade.RevisionDate, returnedTrade.RevisionDate);
            Assert.Equal(trade.DealName, returnedTrade.DealName);
            Assert.Equal(trade.DealType, returnedTrade.DealType);
            Assert.Equal(trade.SourceListId, returnedTrade.SourceListId);
            Assert.Equal(trade.Side, returnedTrade.Side);
        }

        // Test pour GetTradeById - Cas où aucune entité Trade n'est trouvée
        [Fact]
        public async Task GetTradeById_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est de type NotFoundResult
            Assert.Equal(404, notFoundResult.StatusCode);  // Vérifie que le code d'état est 404 (Not Found)
        }

        // Test pour GetTradeById - Cas où une exception est lancée
        [Fact]
        public async Task GetTradeById_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est de type ObjectResult
            Assert.Equal((int)HttpStatusCode.InternalServerError, actionResult.StatusCode);  // Vérifie que le code d'état est 500 (Internal Server Error)
            Assert.Contains("An error occurred while retrieving the Trade ID", actionResult.Value as string);  // Vérifie que le message d'erreur contient le texte attendu
        }
        // Test pour GetAllTrades - Cas où des entités Trade sont trouvées et retournées avec succès
        [Fact]
        public async Task GetAllTrades_ShouldReturnOk_WhenTradesExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            var curves = new List<Trade>
            {
             new Trade {  TradeId = 1,
                Account = "OldAccount",
                AccountType = "OldType",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 15.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "OldSecurity",
                TradeStatus = "OldStatus",
                Trader = "OldTrader",
                Benchmark = "OldBenchmark",
                Book = "OldBook",
                CreationName = "OldCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "OldRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "OldDeal",
                DealType = "OldType",
                SourceListId = "OldList",
                Side = "Sell" },
            new Trade { TradeId = 1,
                Account = "NewAccount",
                AccountType = "NewType",
                BuyQuantity = 200.0,
                SellQuantity = 100.0,
                BuyPrice = 12.0,
                SellPrice = 18.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "NewSecurity",
                TradeStatus = "NewStatus",
                Trader = "NewTrader",
                Benchmark = "NewBenchmark",
                Book = "NewBook",
                CreationName = "NewCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "NewRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "NewDeal",
                DealType = "NewType",
                SourceListId = "NewList",
                Side = "Buy"}
             };

            _tradeRepositoryMock.Setup(r => r.GetAllTradesAsync()).ReturnsAsync(curves);  // Configure le mock pour retourner la liste de courbes

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllTrades();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est du type OkObjectResult (HTTP 200 OK)
            var returnedTrades = Assert.IsType<List<Trade>>(actionResult.Value);  // Vérifie que la valeur retournée est du type List<Trade>
            Assert.Equal(curves.Count, returnedTrades.Count);  // Vérifie que le nombre de courbes retournées est correct
        }

        // Test pour GetAllTrades - Cas où aucune entité Trade n'est trouvée
        [Fact]
        public async Task GetAllTrades_ShouldReturnNotFound_WhenNoTradesExist()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _tradeRepositoryMock.Setup(r => r.GetAllTradesAsync()).ReturnsAsync(new List<Trade>());  // Configure le mock pour retourner une liste vide

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllTrades();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est du type NotFoundResult (HTTP 404 Not Found)
        }

        // Test pour GetAllTrades - Cas où une exception est lancée
        [Fact]
        public async Task GetAllTrades_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange - Prépare les objets nécessaires pour le test
            _tradeRepositoryMock.Setup(r => r.GetAllTradesAsync()).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act - Exécute la méthode à tester
            var result = await _controller.GetAllTrades();

            // Assert - Vérifie que le résultat est correct
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est du type ObjectResult (HTTP 500 Internal Server Error)
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code de statut est 500
            Assert.Equal("An error occurred while retrieving all Trades", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }
        // Test pour UpdateTrade - La mise à jour réussit
        [Fact]
        public async Task UpdateTrade_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var originalTrade = new Trade
            {
                TradeId = 1,
                Account = "OldAccount",
                AccountType = "OldType",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 15.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "OldSecurity",
                TradeStatus = "OldStatus",
                Trader = "OldTrader",
                Benchmark = "OldBenchmark",
                Book = "OldBook",
                CreationName = "OldCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "OldRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "OldDeal",
                DealType = "OldType",
                SourceListId = "OldList",
                Side = "Sell"
            };
            var updatedTrade = new Trade
            {
                TradeId = 1,
                Account = "NewAccount",
                AccountType = "NewType",
                BuyQuantity = 200.0,
                SellQuantity = 100.0,
                BuyPrice = 12.0,
                SellPrice = 18.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "NewSecurity",
                TradeStatus = "NewStatus",
                Trader = "NewTrader",
                Benchmark = "NewBenchmark",
                Book = "NewBook",
                CreationName = "NewCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "NewRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "NewDeal",
                DealType = "NewType",
                SourceListId = "NewList",
                Side = "Buy"
            };

            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync(originalTrade);
            _tradeRepositoryMock.Setup(r => r.UpdateTradeAsync(updatedTrade)).ReturnsAsync(updatedTrade);

            // Act
            var result = await _controller.UpdateTrade(1, updatedTrade);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);  // Vérifie que le résultat est de type OkObjectResult
            Assert.Equal(updatedTrade, actionResult.Value);  // Vérifie que la trade mise à jour est correcte
        }

        // Test pour UpdateTrade - ID dans l'URL ne correspond pas à l'ID de l'objet
        [Fact]
        public async Task UpdateTrade_ShouldReturnBadRequest_WhenIdMismatchOccurs()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 2,  // ID différent de celui dans l'URL
                Account = "NewAccount",
                AccountType = "NewType",
                BuyQuantity = 200.0,
                SellQuantity = 100.0,
                BuyPrice = 12.0,
                SellPrice = 18.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "NewSecurity",
                TradeStatus = "NewStatus",
                Trader = "NewTrader",
                Benchmark = "NewBenchmark",
                Book = "NewBook",
                CreationName = "NewCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "NewRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "NewDeal",
                DealType = "NewType",
                SourceListId = "NewList",
                Side = "Buy"
            };

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);  // Vérifie que le résultat est de type BadRequestObjectResult
            var errors = Assert.IsType<SerializableError>(badRequestResult.Value);  // Vérifie que la réponse contient des erreurs de validation
            Assert.True(errors.ContainsKey("IdMismatch"));  // Vérifie que l'erreur "IdMismatch" est présente
            var errorMessages = errors["IdMismatch"] as string[];
            Assert.Contains("The trade ID in the URL does not match the ID in the trade object.", errorMessages);  // Vérifie que le message d'erreur est correct
        }

        // Test pour UpdateTrade - L'élément à mettre à jour n'existe pas
        [Fact]
        public async Task UpdateTrade_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 1,
                Account = "NewAccount",
                AccountType = "NewType",
                BuyQuantity = 200.0,
                SellQuantity = 100.0,
                BuyPrice = 12.0,
                SellPrice = 18.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "NewSecurity",
                TradeStatus = "NewStatus",
                Trader = "NewTrader",
                Benchmark = "NewBenchmark",
                Book = "NewBook",
                CreationName = "NewCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "NewRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "NewDeal",
                DealType = "NewType",
                SourceListId = "NewList",
                Side = "Buy"
            };
            _tradeRepositoryMock.Setup(r => r.UpdateTradeAsync(trade)).ReturnsAsync((Trade)null);  // Configure le mock pour retourner null lorsque la trade n'existe pas

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est de type NotFoundResult
            Assert.Equal(404, notFoundResult.StatusCode);  // Vérifie que le code d'état est 404 (Not Found)
        }

        // Test pour UpdateTrade - Une exception est levée
        [Fact]
        public async Task UpdateTrade_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var trade = new Trade
            {
                TradeId = 1,
                Account = "NewAccount",
                AccountType = "NewType",
                BuyQuantity = 200.0,
                SellQuantity = 100.0,
                BuyPrice = 12.0,
                SellPrice = 18.0,
                TradeDate = DateTime.UtcNow,
                TradeSecurity = "NewSecurity",
                TradeStatus = "NewStatus",
                Trader = "NewTrader",
                Benchmark = "NewBenchmark",
                Book = "NewBook",
                CreationName = "NewCreator",
                CreationDate = DateTime.UtcNow,
                RevisionName = "NewRevisor",
                RevisionDate = DateTime.UtcNow,
                DealName = "NewDeal",
                DealType = "NewType",
                SourceListId = "NewList",
                Side = "Buy"
            };

            _tradeRepositoryMock.Setup(r => r.UpdateTradeAsync(trade)).ThrowsAsync(new Exception("Database error"));  // Configure le mock pour lancer une exception

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est de type ObjectResult
            Assert.Equal(500, actionResult.StatusCode);  // Vérifie que le code d'état est 500 (Internal Server Error)
            Assert.Equal("An error occurred while retrieving the Trade ID", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }

        // Test pour DeleteTrade - Suppression réussie
        [Fact]
        public async Task DeleteTrade_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            int tradeId = 1;
            _tradeRepositoryMock.Setup(r => r.DeleteTradeAsync(tradeId)).ReturnsAsync(true);  // Configure le mock pour retourner true lors de la suppression réussie

            // Act
            var result = await _controller.DeleteTrade(tradeId);

            // Assert
            Assert.IsType<NoContentResult>(result);  // Vérifie que le résultat est de type NoContentResult
        }

        // Test pour DeleteTrade - Trade non trouvé
        [Fact]
        public async Task DeleteTrade_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            int tradeId = 1;
            _tradeRepositoryMock.Setup(r => r.DeleteTradeAsync(tradeId)).ReturnsAsync(false);  // Configure le mock pour retourner false si la trade n'existe pas

            // Act
            var result = await _controller.DeleteTrade(tradeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);  // Vérifie que le résultat est de type NotFoundResult
            Assert.Equal(404, (result as NotFoundResult).StatusCode);  // Vérifie que le code d'état est 404 (Not Found)
        }

        // Test pour DeleteTrade - Exception levée
        [Fact]
        public async Task DeleteTrade_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int tradeId = 1;

            _tradeRepositoryMock.Setup(r => r.DeleteTradeAsync(tradeId))
                .ThrowsAsync(new Exception("Delete failed"));  // Configure le mock pour lancer une exception lors de la suppression

            // Act
            var result = await _controller.DeleteTrade(tradeId);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);  // Vérifie que le résultat est de type ObjectResult
            Assert.Equal((int)HttpStatusCode.InternalServerError, actionResult.StatusCode);  // Vérifie que le code d'état est 500 (Internal Server Error)
            Assert.Equal("An error occurred while retrieving the Trade ID", actionResult.Value);  // Vérifie que le message d'erreur est correct
        }
    }
}
