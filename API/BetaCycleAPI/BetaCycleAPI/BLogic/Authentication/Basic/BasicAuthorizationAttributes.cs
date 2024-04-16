using Microsoft.AspNetCore.Authorization;

namespace BetaCycleAPI.BLogic.Authentication.Basic
{
    public class BasicAuthorizationAttributes : AuthorizeAttribute

    {
        public BasicAuthorizationAttributes() { Policy = "BasicAuthentication"; }
    }
}
