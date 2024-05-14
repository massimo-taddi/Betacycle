using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public ProductsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<(int,IEnumerable<Product>)>> GetProducts([FromQuery] ProductSpecParams @params)
        {
            List<Product> res = [];
            int productCount = 0;
            List<ProductDescription> test = [];
            switch (@params.Sort)
            {
                case "Desc":
                    if(@params.Search == null)
                    {
                        @params.Search = "";
                    }
                    res = await (from product in _context.Products
                                 join pmpd in _context.ProductModelProductDescriptions on product.ProductModelId equals pmpd.ProductModelId
                                 join descr in _context.ProductDescriptions on pmpd.ProductDescriptionId equals descr.ProductDescriptionId
                                 where product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)
                                 select product).Distinct()
                                                .OrderByDescending(p => p.ListPrice)
                                                //.Skip((@params.PageIndex - 1) * @params.PageSize)
                                                //.Take(@params.PageSize)
                                                .ToListAsync();
                    productCount = res.Count();
                    res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                    break;
                case "Asc":
                    if (@params.Search == null)
                    {
                        @params.Search = "";
                    }
                    res = await (from product in _context.Products
                                 join pmpd in _context.ProductModelProductDescriptions on product.ProductModelId equals pmpd.ProductModelId
                                 join descr in _context.ProductDescriptions on pmpd.ProductDescriptionId equals descr.ProductDescriptionId
                                 where product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search) 
                                 select product).Distinct()
                                                .OrderBy(p => p.ListPrice)
                                                //.Skip((@params.PageIndex - 1) * @params.PageSize)                          
                                                //.Take(@params.PageSize)
                                                .ToListAsync();
                    productCount = res.Count();
                    res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                    break;
                //test per prendere la descrizioni dei prodotti
                case "test":
                    res = await (from product in _context.Products
                                 join pmpd in _context.ProductModelProductDescriptions on product.ProductModelId equals pmpd.ProductModelId
                                 join descr in _context.ProductDescriptions on pmpd.ProductDescriptionId equals descr.ProductDescriptionId
                                 where product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)
                                 select product).Distinct()
                                                .OrderByDescending(p => p.ListPrice)
                                                //.Skip((@params.PageIndex - 1) * @params.PageSize)
                                                //.Take(@params.PageSize)
                                                .ToListAsync();
                    productCount = res.Count();
                    res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();






                    //test = await (from product in _context.ProductDescriptions
                    //              join pmpd in _context.ProductModelProductDescriptions on product.ProductDescriptionId equals pmpd.ProductModelId
                    //              join descr in _context.ProductDescriptions on pmpd.ProductDescriptionId equals descr.ProductDescriptionId
                    //              where product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)
                    //              select product).Distinct()
                    //                            .OrderByDescending(p => p.ListPrice)
                    //                            //.Skip((@params.PageIndex - 1) * @params.PageSize)
                    //                            //.Take(@params.PageSize)
                    //                            .ToListAsync();



                    
                    break;

                default:
                    return BadRequest();
            }
            return (productCount, res);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            if(product.ProductModelId !=null) 
            {

                product.ProductModel = await _context.ProductModels.FindAsync(product.ProductModelId);
                if(product.ProductCategoryId != null) 
                {
                    product.ProductModel.ProductModelProductDescriptions = await _context.ProductModelProductDescriptions.Where(p => p.ProductModelId == product.ProductModel.ProductModelId).ToListAsync();
                    product.ProductCategory = await _context.ProductCategories.FindAsync(product.ProductCategoryId);
                }
                else
                {
                    product.ProductModel.ProductModelProductDescriptions = null;
                    product.ProductCategory = null;
                }
            }else
            {
                product.ProductModel = null;
            }


            return product;
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
        [Authorize]
        [HttpGet]
        [Route("Ncategories")]
        public async Task<ActionResult<(int,IEnumerable<ProductCategory>)>> GetNproductCategories([FromQuery] ProductSpecParams @params)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            int categoryNumber = 0;
            List<ProductCategory> res = [];
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin") 
                return BadRequest();

            switch (@params.Sort)
            {
                case "Desc":
                    res = await (from category in _context.ProductCategories
                                 where category.Name.Contains(@params.Search)
                                 select category).OrderBy(c=>c.Name).ToListAsync();
                    categoryNumber = res.Count();
                    break;
                case "Asc":
                    res = await (from category in _context.ProductCategories
                                 where category.Name.Contains(@params.Search)
                                 select category).OrderByDescending(c => c.Name).ToListAsync();
                    categoryNumber = res.Count();
                    break;
            }

            return (categoryNumber,res) ;
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
        [Authorize]
        [HttpGet]
        [Route("Nmodels")]
        public async Task<ActionResult<(int,IEnumerable<ProductModel>)>> GetNProductModels([FromQuery] ProductSpecParams @params)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            List<ProductModel> productModels = new List<ProductModel>();
            int modelCount = 0;
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin") return BadRequest();


            switch (@params.Sort)
            {
                //aggiungere i search
                case "Desc":
                    productModels = await _context.ProductModels.OrderBy(product => product.ModifiedDate).ToListAsync();
                    modelCount = productModels.Count();
                    break;
                //aggiungere il search
                case "Asc":
                    productModels = await _context.ProductModels.OrderByDescending(product => product.ModifiedDate).ToListAsync();
                    modelCount = productModels.Count();
                    break;
            }
            

            
            productModels = productModels.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
            
            return (modelCount,productModels);
        }
        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        
        public async Task<IActionResult> PutProduct(int id, ProductForm productForm)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }

            Product getRowGuidProduct = await _context.Products.FindAsync(id);
            _context.Products.Entry(getRowGuidProduct).State = EntityState.Detached;

            var product = new Product()
            {
                ProductId=id,
                Name = productForm.Name,
                ProductNumber = productForm.ProductNumber,
                Color = productForm.Color,
                StandardCost = productForm.StandardCost,
                ListPrice = productForm.ListPrice,
                Size = productForm.Size,
                Weight = productForm.Weight,
                ProductCategoryId = productForm.ProductCategoryId,
                ProductModelId = productForm.ProductModelId,    
                SellStartDate = productForm.SellStartDate,
                SellEndDate = productForm.SellEndDate,
                DiscontinuedDate = productForm.DiscontinuedDate,
                ThumbNailPhoto = productForm.ThumbNailPhoto,
                ThumbnailPhotoFileName = productForm.ThumbnailPhotoFileName,
                ModifiedDate = DateTime.Now,
                Rowguid = getRowGuidProduct.Rowguid,

            };
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> PostProduct(ProductForm productForm)
        {

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
                return BadRequest();
            var product = new Product()
            {
                Name = productForm.Name,
                ProductNumber = productForm.ProductNumber,
                Color = productForm.Color,
                StandardCost = productForm.StandardCost,
                ListPrice = productForm.ListPrice,
                Size = productForm.Size,
                Weight = productForm.Weight,
                ProductCategoryId = productForm.ProductCategoryId,
                ProductModelId = productForm.ProductModelId,
                SellStartDate = productForm.SellStartDate,
                SellEndDate = productForm.SellEndDate,
                DiscontinuedDate = productForm.DiscontinuedDate,
                ThumbNailPhoto = productForm.ThumbNailPhoto,
                ThumbnailPhotoFileName = productForm.ThumbnailPhotoFileName,
                ModifiedDate = DateTime.Now
            };


            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
                return BadRequest();
            var product = await _context.Products.FindAsync(id);
            Console.WriteLine(product);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
