using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly ILogger<TradeController> _logger;

        public TradeController(ITradeRepository tradeRepository, ILogger<TradeController> logger)
        {
            _tradeRepository = tradeRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrade([FromBody] Trade trade)
        {
            // Validation de l'objet Trade
            if (trade == null)
            {
                _logger.LogWarning("CreateTrade: Trade object is null.");
                return BadRequest("Trade cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateTrade: ModelState is invalid. Errors: {@Errors}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var createdTrade = await _tradeRepository.CreateTradeAsync(trade);
                _logger.LogInformation("CreateTrade: Trade created successfully with ID {TradeId}.", createdTrade.TradeId);
                return CreatedAtAction(nameof(GetTradeById), new { id = createdTrade.TradeId }, createdTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTrade: An error occurred while creating the trade.");
                return StatusCode(500, "An error occurred while retrieving the Trade ID");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeById(int id)
        {
            try
            {
                var trade = await _tradeRepository.GetTradeByIdAsync(id);
                if (trade == null)
                {
                    _logger.LogWarning("GetTradeById: No trade found with ID {TradeId}.", id);
                    return NotFound();
                }

                _logger.LogInformation("GetTradeById: Successfully retrieved trade with ID {TradeId}.", id);
                return Ok(trade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTradeById: An error occurred while retrieving the trade with ID {TradeId}.", id);
                return StatusCode(500, "An error occurred while retrieving the Trade ID");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrades()
        {
            try
            {
                var trades = await _tradeRepository.GetAllTradesAsync();
                if (trades == null || !trades.Any())
                {
                    _logger.LogInformation("No Trades found.");
                    return NotFound();
                }
                _logger.LogInformation("GetAllTrades: Successfully retrieved all trades.");
                return Ok(trades);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllTrades: An error occurred while retrieving all trades.");
                return StatusCode(500, "An error occurred while retrieving all Trades");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrade(int id, [FromBody] Trade trade)
        {
            // Validation de l'objet Trade et de l'ID
            if (trade == null)
            {
                _logger.LogWarning("UpdateTrade: Trade object is null.");
                return BadRequest("Trade cannot be null.");
            }

            if (id != trade.TradeId)
            {
                _logger.LogWarning("UpdateTrade: Trade ID mismatch. URL ID: {UrlId}, Trade ID: {TradeId}.", id, trade.TradeId);
                ModelState.AddModelError("IdMismatch", "The trade ID in the URL does not match the ID in the trade object.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateTrade: ModelState is invalid. Errors: {@Errors}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTrade = await _tradeRepository.UpdateTradeAsync(trade);
                if (updatedTrade == null)
                {
                    _logger.LogWarning("UpdateTrade: No trade found with ID {TradeId} for update.", id);
                    return NotFound();
                }

                _logger.LogInformation("UpdateTrade: Trade with ID {TradeId} updated successfully.", id);
                return Ok(updatedTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateTrade: An error occurred while updating the trade with ID {TradeId}.", id);
                return StatusCode(500, "An error occurred while retrieving the Trade ID");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            try
            {
                var result = await _tradeRepository.DeleteTradeAsync(id);
                if (!result)
                {
                    _logger.LogWarning("DeleteTrade: No trade found with ID {TradeId} for deletion.", id);
                    return NotFound();
                }

                _logger.LogInformation("DeleteTrade: Trade with ID {TradeId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteTrade: An error occurred while deleting the trade with ID {TradeId}.", id);
                return StatusCode(500, "An error occurred while retrieving the Trade ID");
            }
        }
    }
}
