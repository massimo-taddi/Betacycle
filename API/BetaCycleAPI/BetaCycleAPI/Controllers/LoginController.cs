using Microsoft.AspNetCore.Mvc;
using BetaCycleAPI.Models;
using BetaCycleAPI.BLogic.Authentication.Basic;
using BetaCycleAPI.BLogic.Authentication;
using BetaCycleAPI.Models.Enums;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace BetaCycleAPI.Controllers {

    [ApiController]
    [Route("app/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login(LoginCredentials credentials)
        {
            try
            {
                // Check if passed credentials are valid in the DB's stored credentials
                var result = await CredentialsDBChecker.ValidateLogin(credentials.Username, credentials.Password);
                switch(result)
                {
                    case DBCheckResponse.NotFound:
                        return NotFound();
                    case DBCheckResponse.FoundNotMigrated:
                        return Ok("notMigrated");
                    // An admin is already migrated by default
                    case DBCheckResponse.FoundMigrated:
                        return Ok("migrated");
                    case DBCheckResponse.FoundAdmin:
                        return Ok("admin");
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return BadRequest();
        }

    }
}
