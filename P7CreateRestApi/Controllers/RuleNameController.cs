using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly ILogger<RuleNameController> _logger; 

        public RuleNameController(IRuleRepository ruleRepository, ILogger<RuleNameController> logger)
        {
            _ruleRepository = ruleRepository;
            _logger = logger; 
        }

        // Création d'un RuleName
        [HttpPost]
        public async Task<IActionResult> CreateRuleName([FromBody] RuleName ruleName)
        {
            if (ruleName == null)
            {
                _logger.LogWarning("CreateRuleName was called with a null RuleName object.");
                return BadRequest("RuleName cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("CreateRuleName was called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var createdRuleName = await _ruleRepository.CreateRuleNameAsync(ruleName);
                _logger.LogInformation("RuleName with ID {Id} created successfully.", createdRuleName.Id);
                return CreatedAtAction(nameof(GetRuleNameById), new { id = createdRuleName.Id }, createdRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the RuleName.");
                return StatusCode(500, "An error occurred while retrieving all RuleNames");
            }
        }

        // Récupération d'un RuleName par ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRuleNameById(int id)
        {
            try
            {
                var ruleName = await _ruleRepository.GetRuleNameByIdAsync(id);
                if (ruleName == null)
                {
                    _logger.LogWarning("No RuleName found with ID {Id}.", id);
                    return NotFound($"No RuleName found with ID {id}.");
                }

                _logger.LogInformation("RuleName with ID {Id} retrieved successfully.", id);
                return Ok(ruleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the RuleName with ID {Id}.", id);
                return StatusCode(500, "An error occurred while retrieving all RuleNames");
            }
        }

        // Récupération de tous les RuleNames
        [HttpGet]
        public async Task<IActionResult> GetAllRuleNames()
        {
            try
            {
                var ruleNames = await _ruleRepository.GetAllRuleNamesAsync();
                if (ruleNames == null || !ruleNames.Any())
                {
                    _logger.LogInformation("No RuleName found.");
                    return NotFound();
                }
                _logger.LogInformation("Retrieved all RuleNames successfully.");
                return Ok(ruleNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all RuleNames.");
                return StatusCode(500, "An error occurred while retrieving all RuleNames");
            }
        }

        // Mise à jour d'un RuleName par ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRuleName(int id, [FromBody] RuleName ruleName)
        {
            if (ruleName == null)
            {
                _logger.LogWarning("UpdateRuleName was called with a null RuleName object.");
                return BadRequest("RuleName cannot be null.");
            }

            if (id != ruleName.Id)
            {
                _logger.LogWarning("ID mismatch: URL ID {Id} does not match RuleName ID {RuleNameId}.", id, ruleName.Id);
                ModelState.AddModelError("IdMismatch", "The ID in the URL does not match the ID in the RuleName object.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("UpdateRuleName was called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var updatedRuleName = await _ruleRepository.UpdateRuleNameAsync(ruleName);
                if (updatedRuleName == null)
                {
                    _logger.LogWarning("No RuleName found with ID {Id} for update.", id);
                    return NotFound($"No RuleName found with ID {id}.");
                }

                _logger.LogInformation("RuleName with ID {Id} updated successfully.", id);
                return Ok(updatedRuleName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the RuleName with ID {Id}.", id);
                return StatusCode(500, "An error occurred while retrieving all RuleNames");
            }
        }

        // Suppression d'un RuleName par ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            try
            {
                var result = await _ruleRepository.DeleteRuleNameAsync(id);
                if (!result)
                {
                    _logger.LogWarning("No RuleName found with ID {Id} for deletion.", id);
                    return NotFound($"No RuleName found with ID {id}.");
                }

                _logger.LogInformation("RuleName with ID {Id} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the RuleName with ID {Id}.", id);
                return StatusCode(500, "An error occurred while retrieving all RuleNames");
            }
        }
    }
}
