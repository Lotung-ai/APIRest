using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Data;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeRepository _tradeRepository;
        public TradeController(ITradeRepository ruleRepository)
        {
            _tradeRepository = ruleRepository;
        }
        // TODO: Inject Trade service
        [HttpPost]
        public async Task<IActionResult> CreateTrade(Trade trade)
        {
            if (trade == null)
            {
                return BadRequest("Trade cannot be null.");
            }

            var createdTrade = await _tradeRepository.CreateTradeAsync(trade);
            return CreatedAtAction(nameof(GetTradeById), new { id = createdTrade.TradeId }, createdTrade);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTradeById(int id)
        {
            var trade = await _tradeRepository.GetTradeByIdAsync(id);
            if (trade == null)
            {
                return NotFound();
            }

            return Ok(trade);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTrades()
        {
            var trades = await _tradeRepository.GetAllTradesAsync();
            return Ok(trades);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrade(int id, Trade trade)
        {
            if (id != trade.TradeId)
            {
                return BadRequest();
            }

            var updatedTrade = await _tradeRepository.UpdateTradeAsync(trade);
            return Ok(updatedTrade);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            var result = await _tradeRepository.DeleteTradeAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}