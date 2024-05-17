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
    public class CategoryController : Controller
    {

        private readonly AdventureWorksLt2019Context _context;

        public CategoryController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }
        // GET: api/products/categories
        [Authorize]
        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin") return BadRequest();
            return await _context.ProductCategories.ToListAsync();
        }
        // GET: api/products/categories
        //Get a number of product categories
        [HttpGet]
        [Route("Ncategories")]
        public async Task<ActionResult<(int, IEnumerable<ProductCategory>)>> GetNproductCategories([FromQuery] ProductSpecParams @params)
        {
            int categoryNumber = 0;
            List<ProductCategory> res = [];
            switch (@params.Sort)
            {
                case "Desc":
                    if (@params.Search == null)
                    {
                        @params.Search = "";
                    }
                    res = await (from category in _context.ProductCategories
                                 where category.Name.Contains(@params.Search)
                                 select category).OrderBy(c => c.Name).ToListAsync();
                    categoryNumber = res.Count();
                    res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                    break;
                case "Asc":
                    if (@params.Search == null)
                    {
                        @params.Search = "";
                    }
                    res = await (from category in _context.ProductCategories
                                 where category.Name.Contains(@params.Search)
                                 select category).OrderByDescending(c => c.Name).ToListAsync();
                    categoryNumber = res.Count();
                    res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                    break;
                default:
                    return BadRequest();
            }
            return (categoryNumber, res);
        }

    }
}
