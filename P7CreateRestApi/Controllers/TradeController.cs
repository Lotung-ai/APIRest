using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TradeController : ControllerBase
    {
        // TODO: Inject Trade service
        private readonly ITradeRepository _tradeRepository;
        public TradeController(ITradeRepository curveRepository)
        {
            _tradeRepository = curveRepository;
        }
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
        /*  [HttpGet]
          [Route("list")]
          public IActionResult Home()
          {
              // TODO: find all Trade, add to model
              return Ok();
          }

          [HttpGet]
          [Route("add")]
          public IActionResult AddTrade([FromBody]Trade trade)
          {
              return Ok();
          }

          [HttpGet]
          [Route("validate")]
          public IActionResult Validate([FromBody]Trade trade)
          {
              // TODO: check data valid and save to db, after saving return Trade list
              return Ok();
          }

          [HttpGet]
          [Route("update/{id}")]
          public IActionResult ShowUpdateForm(int id)
          {
              // TODO: get Trade by Id and to model then show to the form
              return Ok();
          }

          [HttpPost]
          [Route("update/{id}")]
          public IActionResult UpdateTrade(int id, [FromBody] Trade trade)
          {
              // TODO: check required fields, if valid call service to update Trade and return Trade list
              return Ok();
          }

          [HttpDelete]
          [Route("{id}")]
          public IActionResult DeleteTrade(int id)
          {
              // TODO: Find Trade by Id and delete the Trade, return to Trade list
              return Ok();
          }*/
    }
}