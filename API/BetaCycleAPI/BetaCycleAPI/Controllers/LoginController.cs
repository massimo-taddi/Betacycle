using Microsoft.AspNetCore.Mvc;
using BetaCycleAPI.Models;
using BetaCycleAPI.BLogic.Authentication.Basic;
namespace BetaCycleAPI.Controllers {

    [ApiController]
    [Route("app/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<bool>> Login(LoginCredentials credentials)
        {
            try
            {
                // steps:
            }
            catch (Exception)
            {

                return BadRequest();
            }

            return Ok();
        }

    }
}
