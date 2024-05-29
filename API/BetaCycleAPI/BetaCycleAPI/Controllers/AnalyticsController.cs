using Microsoft.AspNetCore.Mvc;
using BetaCycleAPI.BLogic;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly AdventureWorksLt2019Context _context;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;
        public AnalyticsController (AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _context = context;
            _credentialsContext = credentialsContext;
        }

        [HttpGet]
        public async Task<ActionResult<(int,IEnumerable<Product>)>> getAllProducts ()
        {
            int countProducts = 0;
            List<Product> res = [];


            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }


            res= await _context.Products.ToListAsync();
            countProducts = res.Count();

            return (countProducts,res);

            
        }
    }
}
