using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {

        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public PasswordResetController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
        }

        // PUT api/<PasswordResetController>
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<bool>> PostAsync([FromBody] PwResetCreds reset)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest();
            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            var dbPwd = (_credentialsContext.Credentials.Find(tokenCustomerId).PasswordHash, _credentialsContext.Credentials.Find(tokenCustomerId).SaltHash);
            if (EncryptData.CypherData.DecryptSalt(reset.oldPassword, dbPwd.SaltHash)== dbPwd.PasswordHash)
            {
                // set new pwd
                var customer = await _credentialsContext.Credentials.Where(c => c.CustomerId == tokenCustomerId).FirstOrDefaultAsync();
                var encrypted = EncryptData.CypherData.SaltEncryp(reset.newPassword);
                customer.PasswordHash = encrypted.Key;
                customer.SaltHash = encrypted.Value;
                _credentialsContext.Entry(customer).State = EntityState.Modified;
                try
                {
                    await _credentialsContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return true;
            }
            return false;
        }
    }
}
