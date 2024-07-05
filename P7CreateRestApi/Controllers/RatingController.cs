using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Changement du Route pour inclure 'api/' pour la cohérence des API REST
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ILogger<RatingController> _logger;

        public RatingController(IRatingRepository ratingRepository, ILogger<RatingController> logger)
        {
            _ratingRepository = ratingRepository;
            _logger = logger;
        }

        // Création d'un Rating
        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] Rating rating)
        {
            if (rating == null)
            {
                _logger.LogWarning("CreateRating: Rating object is null");
                return BadRequest("Rating object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateRating: ModelState is invalid");
                return BadRequest(ModelState);
            }

            try
            {
                var createdRating = await _ratingRepository.CreateRatingAsync(rating);
                _logger.LogInformation("CreateRating: Rating created successfully");
                return CreatedAtAction(nameof(GetRatingById), new { id = createdRating.Id }, createdRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateRating: An error occurred while creating the rating");
                return StatusCode(500, "An error occurred while retrieving Rating");
            }
        }

        // Récupération d'un Rating par ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatingById(int id)
        {
            try
            {
                var rating = await _ratingRepository.GetRatingByIdAsync(id);
                if (rating == null)
                {
                    _logger.LogWarning($"GetRatingById: No rating found with ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"GetRatingById: Successfully retrieved rating with ID {id}");
                return Ok(rating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetRatingById: An error occurred while retrieving the rating with ID {id}");
                return StatusCode(500, "An error occurred while retrieving Rating");
            }
        }

        // Récupération de tous les Ratings
        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            try
            {
                var ratings = await _ratingRepository.GetAllRatingsAsync();
                if (ratings == null || !ratings.Any())
                {
                    _logger.LogInformation("No Ratings found.");
                    return NotFound();
                }
                _logger.LogInformation("GetAllRatings: Successfully retrieved all ratings");
                return Ok(ratings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllRatings: An error occurred while retrieving all ratings");
                return StatusCode(500, "An error occurred while retrieving all Ratings");
            }
        }

        // Mise à jour d'un Rating par ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] Rating rating)
        {
            if (rating == null)
            {
                _logger.LogWarning("UpdateRating: Rating object is null");
                return BadRequest("Rating object is null");
            }

            if (id != rating.Id || !ModelState.IsValid)
            {
                if (id != rating.Id)
                {
                    _logger.LogWarning("UpdateRating: Rating ID mismatch");
                    ModelState.AddModelError("IdMismatch", "The rating ID in the URL does not match the ID in the rating object.");
                }

                _logger.LogWarning("UpdateRating: ModelState is invalid");
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRating = await _ratingRepository.UpdateRatingAsync(rating);
                if (updatedRating == null)
                {
                    _logger.LogWarning($"UpdateRating: No rating found with ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"UpdateRating: Rating updated successfully for ID {id}");
                return Ok(updatedRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateRating: An error occurred while updating the rating with ID {id}");
                return StatusCode(500, "UpdateRating: An error occurred while updating the rating with ID");
            }
        }

        // Suppression d'un Rating par ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            try
            {
                var result = await _ratingRepository.DeleteRatingAsync(id);
                if (!result)
                {
                    _logger.LogWarning($"DeleteRating: No rating found with ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"DeleteRating: Rating deleted successfully for ID {id}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteRating: An error occurred while deleting the rating with ID {id}");
                return StatusCode(500, "An error occurred while retrieving Rating");
            }
        }
    }
}
