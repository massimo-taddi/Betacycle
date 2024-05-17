using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : Controller
    {
        private readonly AdventureWorksLt2019Context _context;

        public ModelController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }
        // GET: api/products/models

        [Authorize]
        [HttpGet]
        [Route("models")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductModels()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin") return BadRequest();
            return await _context.ProductModels.ToListAsync();
        }

        //GET:api/products/Nmodels
        //get a number of models

        [HttpGet]
        [Route("Nmodels")]
        public async Task<ActionResult<(int, IEnumerable<ProductModel>)>> GetNProductModels([FromQuery] ProductSpecParams @params)
        {

            List<ProductModel> productModels = new List<ProductModel>();
            int modelCount = 0;
            switch (@params.Sort)
            {
                case "Desc":
                    if (@params.Search == null)
                    {
                        @params.Search = "";
                    }
                    productModels = await _context.ProductModels.Where(product => product.Name.Contains(@params.Search)).OrderBy(product => product.ModifiedDate).ToListAsync();
                    modelCount = productModels.Count();
                    productModels = productModels.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                    break;

                case "Asc":
                    if (@params.Search == null)
                    {
                        @params.Search = "";
                    }
                    productModels = await _context.ProductModels.OrderByDescending(product => product.ModifiedDate).ToListAsync();
                    modelCount = productModels.Count();
                    productModels = productModels.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                    break;
                default:
                    return BadRequest();
            }
            return (modelCount, productModels);
        }
    }
}
