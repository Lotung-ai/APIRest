using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models;

namespace P7CreateRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {             
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //TODO: implement the UserManager from Identity to validate User and return a security token.
            return Unauthorized();
        }            
    }
}