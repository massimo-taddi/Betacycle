using BetaCycleAPI.BLogic;
using BetaCycleAPI.BLogic.Authentication;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using BetaCycleAPI.Models.Enums;
using BetaCycleAPI.Models.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BetaCycleAPI.Controllers
{
    [Route("app/[controller]")]
    [ApiController]
    public class LoginJwtController : ControllerBase
    {
        private JwtSettings _jwtSettings;
        private readonly AdventureWorksLt2019Context _context;

        public LoginJwtController(JwtSettings jwtSettings, AdventureWorksLt2019Context context)
        {
            _jwtSettings = jwtSettings;
            _context = context;
        }

        /// <summary>
        /// Generates a token for a POST call
        /// </summary>
        /// <param name="loginCredentials">The credentials that will be checked and used for the generation of the token</param>
        /// <returns>A <c>Result</c>, the value of which is determined by checking the DB for credentials</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateToken(LoginCredentials loginCredentials)
        {
            // qui si controlla sul db
            var dbCheckResult = await CredentialsDBChecker.ValidateLogin(loginCredentials.Username.ToLower(), loginCredentials.Password);
            switch(dbCheckResult)
            {
                case DBCheckResponse.FoundMigrated:
                    var token = generateJwtToken(loginCredentials.Username, false);
                    return Ok(new { token });
                case DBCheckResponse.FoundAdmin:
                    token = generateJwtToken(loginCredentials.Username, true);
                    return Ok(new { token });
                case DBCheckResponse.FoundNotMigrated:
                    return NotFound("not migrated");
                case DBCheckResponse.NotFound:
                    return NotFound();

            }
            await DBErrorLogger.WriteExceptionLog(_context, new LoginException("Exception during the JWT login process: the JWT token couldn't be generated."));
            return BadRequest();
        }

        private string generateJwtToken(string username, bool isAdmin)
        {
            var secretKey = _jwtSettings.SecretKey;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, isAdmin ? "admin" : "customer")
                }),
                Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;

        }
    }
}
