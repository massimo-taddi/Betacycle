using BetaCycleAPI.BLogic.Authentication;
using BetaCycleAPI.Models;
using BetaCycleAPI.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginJwtController : ControllerBase
    {
        private JwtSettings _jwtSettings;

        public LoginJwtController(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateToken(LoginCredentials loginCredentials)
        {
            // qui si controlla sul db
            var dbCheckResult = await CredentialsDBChecker.ValidateLogin(loginCredentials.Username.ToLower(), loginCredentials.Password.ToLower());
            switch(dbCheckResult)
            {
                case DBCheckResponse.FoundMigrated:
                    var token = generateJwtToken(loginCredentials.Username);
                    return Ok(new { token });
                case DBCheckResponse.FoundAdmin:
                    token = generateJwtToken(loginCredentials.Username);
                    return Ok(new { token });
                case DBCheckResponse.FoundNotMigrated:
                    return Ok("not migrated");
                case DBCheckResponse.NotFound:
                    return NotFound();

            }
            if (dbCheckResult == DBCheckResponse.FoundMigrated)
            {
                var token = generateJwtToken(loginCredentials.Username);
                return Ok(new { token });
            }
            else
                return BadRequest();
        }

        private string generateJwtToken(string username)
        {
            var secretKey = _jwtSettings.SecretKey;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
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
