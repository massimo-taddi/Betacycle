using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        // GET: api/Category/categories
        //Get all categories
        
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            return await _context.ProductCategories.ToListAsync();
        }
        // GET: api/Category/categories
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

        //GET: api/Category/SingleCategory
        //get single category
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetSingleProductCategory(int id)
        {
            var category = await _context.ProductCategories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        //PUT
        //It modifies a category or a macroCategory
        [HttpPut("{id}")]
        [Authorize]

        public async Task<IActionResult> ModifyCategory(int id,string name,bool discontinued,int? parentCategory)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin") return BadRequest();

            ProductCategory getRowguidProductModel = await _context.ProductCategories.FindAsync(id);
            _context.ProductCategories.Entry(getRowguidProductModel).State = EntityState.Detached;


            ProductCategory res = new ProductCategory()
            {
                ProductCategoryId = id,
                Name = name,
                Discontinued = discontinued,
                ModifiedDate = DateTime.Now,
                Rowguid=getRowguidProductModel.Rowguid,
                ParentProductCategoryId = parentCategory,
            };
            _context.Entry(res).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.Message);
            }

            return NoContent();
        }


        //POST
        //it post a new record in productCategory table
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCategory(int? parentProductCategory,string name)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin") return BadRequest();

            ProductCategory res = new ProductCategory() 
            {
            
                ParentProductCategoryId=parentProductCategory,
                Discontinued=false,
                ModifiedDate = DateTime.Now,
                Name = name,
            };

            _context.ProductCategories.Add(res);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }


        //DELETE 

        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
                return BadRequest();
            var category = await _context.ProductCategories.FindAsync(id);
            _context.ProductCategories.Remove(category);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return NoContent();
        }
    }
}
