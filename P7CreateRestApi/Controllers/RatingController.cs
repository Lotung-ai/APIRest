using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ILogger<RatingController> _logger;

        public RatingController(IRatingRepository ratingRepository, ILogger<RatingController> logger)
        {
            _ratingRepository = ratingRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] Rating rating)
        {
            if (rating == null)
            {
                return BadRequest();  // Assure que BadRequest est renvoyé si rating est null
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Assure que BadRequest est renvoyé si ModelState est invalide
            }

            try
            {
                var createdRating = await _ratingRepository.CreateRatingAsync(rating);
                _logger.LogInformation("Rating created successfully");
                return CreatedAtAction(nameof(GetRatingById), new { id = createdRating.Id }, createdRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the rating");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatingById(int id)
        {
            try
            {
                var rating = await _ratingRepository.GetRatingByIdAsync(id);
                if (rating == null)
                {
                    return NotFound();
                }

                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the rating");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            try
            {
                var ratings = await _ratingRepository.GetAllRatingsAsync();
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all ratings");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> UpdateRating(int id, [FromBody] Rating rating)
        {
            if (rating == null)
            {
                ModelState.AddModelError("Rating", "The rating parameter cannot be null and must have a valid ID.");
                return BadRequest(ModelState);
            }

            if (id != rating.Id || !ModelState.IsValid)
            {
                if (id != rating.Id)
                {
                    ModelState.AddModelError("IdMismatch", "The rating ID in the URL does not match the ID in the rating object.");
                }
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRating = await _ratingRepository.UpdateRatingAsync(rating);
                if (updatedRating == null)
                {
                    return NotFound();
                }

                _logger.LogInformation("Rating updated successfully");
                return Ok(updatedRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the rating");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            try
            {
                var result = await _ratingRepository.DeleteRatingAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                _logger.LogInformation("Rating deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the rating");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
