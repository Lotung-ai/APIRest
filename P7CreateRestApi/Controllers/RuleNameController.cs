using Microsoft.AspNetCore.Mvc;
using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Data;

namespace Dot.Net.WebApi.Controllers
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
        /*[HttpGet]
        [Route("list")]
        public IActionResult Home()
        {
            // TODO: find all RuleName, add to model
            return Ok();
        }

        [HttpGet]
        [Route("add")]
        public IActionResult AddRuleName([FromBody]RuleName trade)
        {
            return Ok();
        }

        [HttpGet]
        [Route("validate")]
        public IActionResult Validate([FromBody]RuleName trade)
        {
            // TODO: check data valid and save to db, after saving return RuleName list
            return Ok();
        }

        [HttpGet]
        [Route("update/{id}")]
        public IActionResult ShowUpdateForm(int id)
        {
            // TODO: get RuleName by Id and to model then show to the form
            return Ok();
        }

        [HttpPost]
        [Route("update/{id}")]
        public IActionResult UpdateRuleName(int id, [FromBody] RuleName rating)
        {
            // TODO: check required fields, if valid call service to update RuleName and return RuleName list
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteRuleName(int id)
        {
            // TODO: Find RuleName by Id and delete the RuleName, return to Rule list
            return Ok();
        }*/
    }
}