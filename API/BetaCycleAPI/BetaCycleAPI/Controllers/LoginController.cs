using Microsoft.AspNetCore.Mvc;
using BetaCycleAPI.Models;
using BetaCycleAPI.BLogic.Authentication.Basic;
namespace BetaCycleAPI.Controllers {

    [ApiController]
    [Route("app/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult Login(LoginCredentials credentials)
        {
            try
            {
                int x = 0;

            }
            catch (Exception)
            {

                return BadRequest();
            }

            return Ok();
        }

    }
}
