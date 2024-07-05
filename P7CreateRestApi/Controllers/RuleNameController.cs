using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleRepository _ruleRepository;
        public RuleNameController(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }
        // TODO: Inject RuleName service
        [HttpPost]
        public async Task<IActionResult> CreateRuleName(RuleName ruleName)
        {
            if (ruleName == null)
            {
                return BadRequest("RuleName cannot be null.");
            }

            var createdRuleName = await _ruleRepository.CreateRuleNameAsync(ruleName);
            return CreatedAtAction(nameof(GetRuleNameById), new { id = createdRuleName.Id }, createdRuleName);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRuleNameById(int id)
        {
            var ruleName = await _ruleRepository.GetRuleNameByIdAsync(id);
            if (ruleName == null)
            {
                return NotFound();
            }

            return Ok(ruleName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRuleNames()
        {
            var ruleNames = await _ruleRepository.GetAllRuleNamesAsync();
            return Ok(ruleNames);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRuleName(int id, RuleName ruleName)
        {
            if (id != ruleName.Id)
            {
                return BadRequest();
            }

            var updatedRuleName = await _ruleRepository.UpdateRuleNameAsync(ruleName);
            return Ok(updatedRuleName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            var result = await _ruleRepository.DeleteRuleNameAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
       
    }
}