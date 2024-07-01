using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurveController : ControllerBase
    {
        // TODO: Inject Curve Point service
        private readonly ICurvePointRepository _curveRepository;
        public CurveController(ICurvePointRepository curveRepository)
        {
            _curveRepository = curveRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCurvePoint(CurvePoint curvePoint)
        {
            if (curvePoint == null)
            {
                return BadRequest("CurvePoint cannot be null.");
            }

            var createdCurvePoint = await _curveRepository.CreateCurvePointAsync(curvePoint);
            return CreatedAtAction(nameof(GetCurvePointById), new { id = createdCurvePoint.Id }, createdCurvePoint);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurvePointById(int id)
        {
            var curvePoint = await _curveRepository.GetCurvePointByIdAsync(id);
            if (curvePoint == null)
            {
                return NotFound();
            }

            return Ok(curvePoint);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCurvePoints()
        {
            var curvePoints = await _curveRepository.GetAllCurvePointsAsync();
            return Ok(curvePoints);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurvePoint(int id, CurvePoint curvePoint)
        {
            if (id != curvePoint.Id)
            {
                return BadRequest();
            }

            var updatedCurvePoint = await _curveRepository.UpdateCurvePointAsync(curvePoint);
            return Ok(updatedCurvePoint);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            var result = await _curveRepository.DeleteCurvePointAsync(id);
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
            return Ok();
        }

        [HttpGet]
        [Route("add")]
        public IActionResult AddCurvePoint([FromBody]CurvePoint curvePoint)
        {
            return Ok();
        }

        [HttpGet]
        [Route("validate")]
        public IActionResult Validate([FromBody]CurvePoint curvePoint)
        {
            // TODO: check data valid and save to db, after saving return bid list
            return Ok();
        }

        [HttpGet]
        [Route("update/{id}")]
        public IActionResult ShowUpdateForm(int id)
        {
            // TODO: get CurvePoint by Id and to model then show to the form
            return Ok();
        }*/

    }
}