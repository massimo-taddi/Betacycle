using BetaCycleAPI.BLogic;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordResetController : ControllerBase
    {

        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;
        private readonly string _siteUrl;
        private readonly JwtSettings _jwtSettings;

        public PasswordResetController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext, string siteUrl, JwtSettings jwtSettings)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
            _siteUrl = siteUrl;
            _jwtSettings = jwtSettings;
        }


        // PUT api/<PasswordResetController>
        [HttpPut]
        [Authorize]
        [Route("loggedin")]
        public async Task<ActionResult<bool>> ResetPwdWhileLoggedIn([FromBody] PwResetCreds reset)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest();
            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            try
            {
                var dbPwd = (_credentialsContext.Credentials.Find(tokenCustomerId).PasswordHash, _credentialsContext.Credentials.Find(tokenCustomerId).SaltHash);
                if (EncryptData.CypherData.DecryptSalt(reset.oldPassword, dbPwd.SaltHash) == dbPwd.PasswordHash)
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
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return false;
        }

        [HttpPut]
        [Authorize]
        [Route("notloggedin")]
        public async Task<IActionResult> ResetPwdWhileNotLoggedIn(string newPwd)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest();
            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            var tokenCustomer = _awContext.Customers.Find((int)tokenCustomerId);
            var newCreds = EncryptData.CypherData.SaltEncryp(newPwd);
            tokenCustomer.PasswordHash = newCreds.Key;
            tokenCustomer.PasswordSalt = newCreds.Value;
            _awContext.Entry(tokenCustomer).State = EntityState.Modified;
            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return Ok();
        }

        // POST api/<PasswordResetController>/forgot
        // pagina pwdforgot: ti chiede di inserire la mail e la manda qua
        [Route("forgot")]
        [HttpPost]
        public async Task<ActionResult<bool>> SendEmail(string email)
        {
            // invia la mail, che deve contenere un link a pwforgot con un token valido tipo 10 minuti con unique_name=mail
            // in pwforgot ho quindi il token, e avro' un singolo form che chiede la nuova pwd. a questo punto facciamo una nuova PUT 
            // in cui aggiorniamo il record dell'utente verificando il token
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("beta89256464@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Reset della password del tuo account Betacycle";

            // genera il token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, "customer")
                }),
                Expires = DateTime.Now.AddMinutes(10),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            string tokenString = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            mail.IsBodyHtml = true;
            mail.Body = $"<a href=\"http://{_siteUrl}:4200/resetforgot?token={tokenString}\">Password reset</a>";

            //password originale betaprova123


            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("beta89256464@gmail.com", "ooriltjjyrjekmvi");
            smtpClient.EnableSsl = true;
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return true;
        }

    }
}
