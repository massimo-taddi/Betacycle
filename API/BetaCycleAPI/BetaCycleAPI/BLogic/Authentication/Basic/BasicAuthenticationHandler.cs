using BetaCycleAPI.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using BetaCycleAPI.BLogic.Authentication;
using BetaCycleAPI.Models.Enums;
using BetaCycleAPI.Models;




namespace BetaCycleAPI.BLogic.Authentication.Basic
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        [Obsolete]
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Append("WWW-Authenticate", "Basic");

            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Autorizzazione mancante"));
            }

            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authorHeaderRegEx = new Regex("Basic (.*)");

            if (!authorHeaderRegEx.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Autorizzazione non valida!"));
            }

            var auth64 = Encoding.UTF8.GetString(
                Convert.FromBase64String(authorHeaderRegEx
                .Replace(authorizationHeader, "$1")));

            var authArraySplit = auth64.Split(Convert.ToChar(":"), 2);
            var authUser = authArraySplit[0];
            var authPassword = authArraySplit.Length > 1 ?
                authArraySplit[1] : throw new Exception("Password NON presente");

            if (string.IsNullOrEmpty(authUser) || string.IsNullOrEmpty(authPassword.Trim()))
            {
                return Task.FromResult(AuthenticateResult.Fail("Username e/o Password NON validi"));
            }
            else
            {
                CredentialsDBChecker dbChecker = new();
                
                switch (dbChecker.ValidateLogin(authUser, authPassword))
                {
                    case DBCheckResponse.NotFound:
                        return Task.FromResult(AuthenticateResult.Fail("Username e/o Password NON validi"));
                        break;
                    case DBCheckResponse.FoundNotMigrated:
                        // ri-crea l'account a partire da questa mail
                        return Task.FromResult(AuthenticateResult.Fail("Account pre-migrazione, da ricreare"));
                        break;
                }
            }

            var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, authArraySplit[0].ToString());
            var claimsMain = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsMain, Scheme.Name)));
        }

    }
}
