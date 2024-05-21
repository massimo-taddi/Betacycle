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
using Microsoft.IdentityModel.Tokens;
using BetaCycleAPI.BLogic;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;
        public ProductsController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _context = context;
            _credentialsContext = credentialsContext;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<(int,IEnumerable<Product>)>> GetProducts([FromQuery] ProductSpecParams @params)
        {
            List<Product> res = [];
            int productCount = 0;
            List<ProductDescription> test = [];
            try
            {
                if (@params.Search == null)
                {
                    @params.Search = "";
                }
                res = await (from product in _context.Products
                             from pmpd in product.ProductModel.ProductModelProductDescriptions
                             where (EF.Functions.FreeText(product.Name, @params.Search) || EF.Functions.FreeText(pmpd.ProductDescription.Description, @params.Search) && product.OnSale)
                             select product).Distinct().ToListAsync();

                productCount = res.Count();
                switch (@params.Sort)
                {
                    case "Desc":
                        res = res.OrderByDescending(p => p.ListPrice).Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                        break;
                    case "Asc":
                        res = res.OrderBy(p => p.ListPrice).Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                        break;
                    //test per prendere la descrizioni dei prodotti
                    case "test":
                        res = res.OrderByDescending(p => p.ListPrice).Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
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
                    foreach(var desc in product.ProductModel.ProductModelProductDescriptions)
                    {
                        desc.ProductDescription = await _context.ProductDescriptions.FindAsync(desc.ProductDescriptionId);
                    }
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


      
        //GET:api/products/Recommendations
       //get recommended products

        [HttpGet]
        [Route("Recommendations")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetRecommendedProducts()
        {
            List<Product> res = [];
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));


            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _context.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            var evaluationScoresMaxHeap = new PriorityQueue<int, float>(new FloatMaxCompare());
            try
            {
                foreach(var prod in await _context.Products.ToListAsync())
                {
                    if(prod.OnSale)
                    {
                        var prediction = RecommendProduct.Predict(new RecommendProduct.ModelInput() { CustomerID = tokenCustomerId, ProductID = prod.ProductId });
                        if(prediction.ProductID != 0F) {
                            evaluationScoresMaxHeap.Enqueue(prod.ProductId, prediction.Score);
                        }
                        else
                        {
                            return await RandomProducts();
                        }
                    }
                }
                for (int i = 0; i < 9; i++)
                {
                    res.Add(await _context.Products.FindAsync(evaluationScoresMaxHeap.Dequeue()));
                }
            }catch(Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return res;
        }
        private class FloatMaxCompare : IComparer<float>
        {
            public int Compare(float x, float y) => y.CompareTo(x);
        }
        [HttpGet]
        [Route("RandomProducts")]
        public async Task<ActionResult<List<Product>>> RandomProducts()
        {
            List<Product> res = [];
            try
            {
                var allProds = await _context.Products.ToListAsync();
                Random rnd = new Random();
                for (int i = 0; i < 9; i++)
                {
                    res.Add(allProds[(int)rnd.Next(allProds.Count)]);
                }
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return res;
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
            catch (DbUpdateConcurrencyException e)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    await DBErrorLogger.WriteExceptionLog(_context, e);
                    return BadRequest();
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
                ModifiedDate = DateTime.Now,
                OnSale = productForm.OnSale
            };


          _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }catch(Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }

            
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
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }catch(Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
