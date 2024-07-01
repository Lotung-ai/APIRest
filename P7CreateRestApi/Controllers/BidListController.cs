using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidRepository _bidRepository;

        public BidListController(IBidRepository bidRepository)
        {
            _bidRepository = bidRepository;
        }
        // 1.1: Impl�mentez l'API RESTFUL pour cr�er une entit� Bid dans le DataRepository
        [HttpPost]
        public async Task<IActionResult> CreateBid([FromBody] BidList bid)
        {
            if (bid == null)
            {
                return BadRequest("Bid object is null");
            }

            var createdBid = await _bidRepository.CreateBidAsync(bid);
            return CreatedAtAction(nameof(GetBidById), new { id = createdBid.BidListId }, createdBid);
        }

        // 1.2: Impl�mentez l'API RESTFUL pour r�cup�rer une entit� Bid
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBidById(int id)
        {
            var bid = await _bidRepository.GetBidByIdAsync(id);
            if (bid == null)
            {
                return NotFound();
            }
            return Ok(bid);
        }

        // 1.3: Impl�mentez l'API RESTFUL pour modifier une entit� Bid
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] BidList bid)
        {
            if (bid == null || bid.BidListId != id)
            {
                return BadRequest("Bid object is null or ID mismatch");
            }

            var existingBid = await _bidRepository.GetBidByIdAsync(id);
            if (existingBid == null)
            {
                return NotFound();
            }

            await _bidRepository.UpdateBidAsync(bid);
            return NoContent();
        }

        // 1.4: Impl�mentez l'API RESTFUL pour supprimer une entit� Bid
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var result = await _bidRepository.DeleteBidAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

      /*  [HttpGet]
        [Route("validate")]
        public IActionResult Validate([FromBody] BidList bidList)
        {
            // TODO: check data valid and save to db, after saving return bid list
            return Ok();
        }

        [HttpGet]
        [Route("update/{id}")]
        public IActionResult ShowUpdateForm(int id)
        {
            return Ok();
        }*/

    }
}