using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Data;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RatingController : ControllerBase
    {

        // TODO: Inject Curve Point service
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRating(Rating rating)
        {
            if (rating == null)
            {
                return BadRequest("Rating cannot be null.");
            }

            var createdRating = await _ratingRepository.CreateRatingAsync(rating);
            return CreatedAtAction(nameof(GetRatingById), new { id = createdRating.Id }, createdRating);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRatingById(int id)
        {
            var rating = await _ratingRepository.GetRatingByIdAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRatings()
        {
            var rating = await _ratingRepository.GetAllRatingsAsync();
            return Ok(rating);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, Rating rating)
        {
            if (id != rating.Id)
            {
                return BadRequest();
            }

            var updatedRating = await _ratingRepository.UpdateRatingAsync(rating);
            return Ok(updatedRating);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var result = await _ratingRepository.DeleteRatingAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        // TODO: Inject Rating service

        /* [HttpGet]
         [Route("list")]
         public IActionResult Home()
         {
             // TODO: find all Rating, add to model
             return Ok();
         }

         [HttpGet]
         [Route("add")]
         public IActionResult AddRatingForm([FromBody]Rating rating)
         {
             return Ok();
         }

         [HttpGet]
         [Route("validate")]
         public IActionResult Validate([FromBody]Rating rating)
         {
             // TODO: check data valid and save to db, after saving return Rating list
             return Ok();
         }

         [HttpGet]
         [Route("update/{id}")]
         public IActionResult ShowUpdateForm(int id)
         {
             // TODO: get Rating by Id and to model then show to the form
             return Ok();
         }

         [HttpPost]
         [Route("update/{id}")]
         public IActionResult UpdateRating(int id, [FromBody] Rating rating)
         {
             // TODO: check required fields, if valid call service to update Rating and return Rating list
             return Ok();
         }

         [HttpDelete]
         [Route("{id}")]
         public IActionResult DeleteRating(int id)
         {
             // TODO: Find Rating by Id and delete the Rating, return to Rating list
             return Ok();
         }*/
    }
}