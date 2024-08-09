using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using P7CreateRestApi.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidRepository _bidRepository;
        private readonly ILogger<BidListController> _logger;

        public BidListController(IBidRepository bidRepository, ILogger<BidListController> logger) 
        {
            _bidRepository = bidRepository;
            _logger = logger;
        }

        // 1.1: Implémentez l'API RESTFUL pour créer une entité Bid dans le DataRepository
        [HttpPost]
        [Authorize(Roles ="User, Admin")]            
        public async Task<IActionResult> CreateBid([FromBody] BidList bid)
        {
            _logger.LogInformation("CreateBid: User is attempting to create a bid.");
            
        
            if (bid == null)
            {
                _logger.LogWarning("CreateBid: Bid object is null");
                return BadRequest("Bid object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateBid: ModelState is invalid");
                return BadRequest(ModelState);  // Renvoie les erreurs de validation des données
            }

            try
            {
                var createdBid = await _bidRepository.CreateBidAsync(bid);
                _logger.LogInformation($"CreateBid: Successfully created Bid with ID {createdBid.BidListId}");
                return CreatedAtAction(nameof(GetBidById), new { id = createdBid.BidListId }, createdBid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateBid: An error occurred while creating the bid");
                return StatusCode(500, "Internal server error");
            }
        }

        // 1.2: Implémentez l'API RESTFUL pour récupérer une entité Bid
        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetBidById(int id)
        {
            try
            {
                var bid = await _bidRepository.GetBidByIdAsync(id);
                if (bid == null)
                {
                    _logger.LogWarning($"GetBidById: No Bid found with ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"GetBidById: Successfully retrieved Bid with ID {id}");
                return Ok(bid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetBidById: An error occurred while retrieving the bid with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
        // Récupération de tous les Bid
        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetAllBids()
        {
            try
            {
                var bids = await _bidRepository.GetAllBidsAsync();
                if (bids == null || !bids.Any())
                {
                    _logger.LogInformation("No Bids found.");
                    return NotFound();
                }
                _logger.LogInformation("GetAllBids: Successfully retrieved all bids");
                return Ok(bids);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllBids: An error occurred while retrieving all bids");
                return StatusCode(500, "An error occurred while retrieving all Bids");
            }
        }
        // 1.3: Implémentez l'API RESTFUL pour modifier une entité Bid
        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] BidList bid)
        {
            if (bid == null || bid.BidListId != id)
            {
                _logger.LogWarning($"UpdateBid: Bid object is null or ID mismatch");
                return BadRequest("Bid object is null or ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"UpdateBid: ModelState is invalid for ID {id}");
                return BadRequest(ModelState);  // Renvoie les erreurs de validation des données
            }

            try
            {
                var updatedBid= await _bidRepository.UpdateBidAsync(bid);
                if (updatedBid == null)
                {
                    _logger.LogWarning($"UpdateBid: No Bid found with ID {id}");
                    return NotFound();
                }
               
                _logger.LogInformation($"UpdateBid: Successfully updated Bid with ID {id}");
                return Ok(updatedBid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateBid: An error occurred while updating the bid with ID {id}");
                return StatusCode(500, "An error occurred while updating a bid");
            }
        }

        // 1.4: Implémentez l'API RESTFUL pour supprimer une entité Bid
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            _logger.LogInformation("CreateBid: User is attempting to create a bid.");
            try
            {
                var result = await _bidRepository.DeleteBidAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"DeleteBid: No Bid found with ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"DeleteBid: Successfully deleted Bid with ID {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteBid: An error occurred while deleting the bid with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
