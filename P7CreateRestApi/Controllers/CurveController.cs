using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CurveController : ControllerBase
    {
        private readonly ICurvePointRepository _curveRepository;
        private readonly ILogger<CurveController> _logger;

        public CurveController(ICurvePointRepository curveRepository, ILogger<CurveController> logger)
        {
            _curveRepository = curveRepository;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> CreateCurvePoint(CurvePoint curvePoint)
        {
            _logger.LogInformation("CreateCurvePoint called with curvePoint: {@CurvePoint}", curvePoint);

            if (curvePoint == null)
            {
                _logger.LogWarning("Received null CurvePoint in CreateCurvePoint.");
                return BadRequest("CurvePoint cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid data in CreateCurvePoint.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating CurvePoint with data: {@CurvePoint}", curvePoint);
                var createdCurvePoint = await _curveRepository.CreateCurvePointAsync(curvePoint);
                _logger.LogInformation("Successfully created CurvePoint with ID: {Id}", createdCurvePoint.Id);
                return CreatedAtAction(nameof(GetCurvePointById), new { id = createdCurvePoint.Id }, createdCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating CurvePoint.");
                return StatusCode(500, "An error occurred while creating the CurvePoint.");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetCurvePointById(int id)
        {
            _logger.LogInformation("GetCurvePointById called with ID: {Id}", id);

            try
            {
                var curvePoint = await _curveRepository.GetCurvePointByIdAsync(id);
                if (curvePoint == null)
                {
                    _logger.LogWarning("CurvePoint with ID {Id} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Retrieved CurvePoint with ID: {Id}", id);
                return Ok(curvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving CurvePoint with ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the CurvePoint.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetAllCurvePoints()
        {
            _logger.LogInformation("GetAllCurvePoints called.");

            try
            {
                var curvePoints = await _curveRepository.GetAllCurvePointsAsync();
                if (curvePoints == null || !curvePoints.Any())
                {
                    _logger.LogInformation("No CurvePoints found.");
                    return NotFound();
                }
                _logger.LogInformation("Retrieved {Count} CurvePoints.", curvePoints.Count());
                return Ok(curvePoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all CurvePoints.");
                return StatusCode(500, "An error occurred while retrieving all CurvePoints.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> UpdateCurvePoint(int id, CurvePoint curvePoint)
        {
            _logger.LogInformation("UpdateCurvePoint called with ID: {Id} and data: {@CurvePoint}", id, curvePoint);

           if (curvePoint == null)
            {
                _logger.LogWarning("UpdateCurvePoint: CurvePoint object is null");
                return BadRequest("CurvePoint object is null");
            }

            if (id != curvePoint.Id || !ModelState.IsValid)
            {
                if (id != curvePoint.Id)
                {
                    _logger.LogWarning("UpdateCurvePoint: CurvePoint ID mismatch");
                    ModelState.AddModelError("IdMismatch", "The curvePoint ID in the URL does not match the ID in the curve object.");
                }

                _logger.LogWarning("UpdateCurvePoint: ModelState is invalid");
                return BadRequest(ModelState);
            }

            try
            {
                var updatedCurvePoint = await _curveRepository.UpdateCurvePointAsync(curvePoint);
                if (updatedCurvePoint == null)
                {
                    _logger.LogWarning($"UpdateCurvePoint: No curvePoint found with ID {id}");
                    return NotFound();
                }

                _logger.LogInformation($"UpdateCurvePoint: CurvePoint updated successfully for ID {id}");
                return Ok(updatedCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateCurvePoint: An error occurred while updating the curvePoint with ID {id}");
                return StatusCode(500, "UpdateCurvePoint: An error occurred while updating the curvePoint with ID");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            _logger.LogInformation("DeleteCurvePoint called with ID: {Id}", id);

            try
            {
                var result = await _curveRepository.DeleteCurvePointAsync(id);
                if (!result)
                {
                    _logger.LogWarning("CurvePoint with ID {Id} not found for deletion.", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully deleted CurvePoint with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting CurvePoint with ID: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the CurvePoint.");
            }
        }
    }
}
