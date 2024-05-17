﻿using System;
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
                                 where (product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)) && product.OnSale
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
                                 where (product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)) && product.OnSale
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
                                 where (product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)) && product.OnSale
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
            for(int i = 0; i < 9; i++)
            {
                res.Add(await _context.Products.FindAsync(evaluationScoresMaxHeap.Dequeue()));
            }
            return res;
        }
        private class FloatMaxCompare : IComparer<float>
        {
            public int Compare(float x, float y) => y.CompareTo(x);
        }
        [HttpGet]
        [Route("RandomProducts")]
        private async Task<List<Product>> RandomProducts()
        {
            List<Product> res = [];
            var allProds = await _context.Products.ToListAsync();
            Random rnd = new Random();
            for (int i = 0; i < 9; i++)
            {
                res.Add(allProds[(int)rnd.Next(allProds.Count)]);
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
                return BadRequest(e.Message);
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
