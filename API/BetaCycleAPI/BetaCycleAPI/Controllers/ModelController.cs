using BetaCycleAPI.BLogic;
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
        // GET: api/Model/models


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductModels()
        {
            try
            {
                return await _context.ProductModels.ToListAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
        }

        //GET:api/Model/Nmodels
        //get a number of models

        [HttpGet]
        [Route("Nmodels")]
        public async Task<ActionResult<(int, IEnumerable<ProductModel>)>> GetNProductModels([FromQuery] ProductSpecParams @params)
        {

            List<ProductModel> productModels = new List<ProductModel>();
            int modelCount = 0;
            try
            {
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
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return (modelCount, productModels);
        }
        //GET: api/Model/SingleModel
        //get single category

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetSingleModel(int id)
        {
            var category = await _context.ProductModels.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        //PUT:api/Model/ModifyModel/{id}
        //updates a single record in the product model table

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ModifyModel(int id, string name, bool discontinued)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }

            ProductModel getRowguidProductModel = await _context.ProductModels.FindAsync(id);
            _context.ProductModels.Entry(getRowguidProductModel).State = EntityState.Detached;

            ProductModel res = new ProductModel()
            {
                ProductModelId = id,
                Name = name,
                ModifiedDate = DateTime.Now,
                Rowguid = getRowguidProductModel.Rowguid,
                Discontinued = discontinued,

            };
            _context.Entry(res).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }

            return NoContent();
        }


        //POST:
        //it insert a record in the productModel Table
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostAModel(string name, bool discontinued)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }
            ProductModel res = new ProductModel()
            {

                Name = name,
                ModifiedDate = DateTime.Now,
                Discontinued = discontinued,

            };
            _context.ProductModels.Add(res);

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return NoContent();
        }
        //DELETE
        //Delete a single record from the ProductModel Table
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }
            var res = await _context.ProductModels.FindAsync(id);


            _context.ProductModels.Remove(res);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return NoContent();
        }
    }
}
