﻿using BetaCycleAPI.BLogic;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using BetaCycleAPI.Models.Cache;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Contexts
        private readonly AdventureWorksLt2019Context _context;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;
        // Cache
        private readonly InMemoryCache<string, IEnumerable<Product>> _cache;
        public ProductsController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext, InMemoryCache<string, IEnumerable<Product>> cache)
        {
            _context = context;
            _credentialsContext = credentialsContext;
            _cache = cache;
        }
        #region Private Methods
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        private class FloatMaxCompare : IComparer<float>
        {
            public int Compare(float x, float y) => y.CompareTo(x);
        }
        #endregion

        #region Public Methods

        // GET: api/Products
        /// <summary>
        /// Cached search for products.
        /// </summary>
        /// <param name="params">The <b>SearchParams</b> for this search query</param>
        /// <returns>An (int, IEnumerable<Product>) tuple containing the total amount of rows for the query and the @params.pageIndex-th @params.pageSize rows</returns>
        [HttpGet]
        public async Task<ActionResult<(int, IEnumerable<Product>)>> GetProducts([FromQuery] ProductSpecParams @params)
        {
            List<Product> res = [];
            int prodCount = 0;
            List<ProductDescription> test = [];
            try
            {
                if (@params.Search == null || @params.Search == "")
                {
                    return BadRequest("Empty search is not allowed.");
                }

                //var allProds = await _context.Products
                //    .Where(prod => ((EF.Functions.FreeText(prod.Name, @params.Search) || prod.ProductModel.ProductModelProductDescriptions.Any(pmpd => (EF.Functions.FreeText(pmpd.ProductDescription.Description, @params.Search) || pmpd.ProductDescription.Description.Contains(@params.Search)))) || (prod.Name.Contains(@params.Search)) && prod.OnSale)).ToListAsync();
                IEnumerable<Product> allProds = [];
                // If item is in cache, fetch it
                if (_cache.HasItem(@params.Search))
                {
                    allProds = _cache.Get(@params.Search)!;
                } else
                {
                    #pragma warning disable EF1002 // Risk of vulnerability to SQL injection. (solved through input sanitation)
                    allProds = await _context.Products.FromSqlRaw($"SELECT p.* FROM [SalesLT].[Product] AS p WHERE (CONTAINS([Name], '\"*{@params.Search.Replace("'", "''").Replace(' ', '*')}*\"') OR FREETEXT([Name], '\"{@params.Search.Replace("'", "''")}\"')) AND p.[OnSale] = 1 UNION SELECT p.* FROM [SalesLT].[Product] AS p INNER JOIN [SalesLT].[ProductModel] AS pm ON p.ProductModelID = pm.ProductModelID INNER JOIN [SalesLT].[ProductModelProductDescription] AS pmpd ON pm.ProductModelID = pmpd.ProductModelID INNER JOIN [SalesLT].[ProductDescription] AS pd ON pmpd.ProductDescriptionID = pd.ProductDescriptionID WHERE (CONTAINS(pd.[Description], '\"*{@params.Search.Replace("'", "''").Replace(' ', '*')}*\"') OR FREETEXT(pd.[Description], '\"{@params.Search.Replace("'", "''")}\"')) AND p.[OnSale] = 1").ToListAsync();
                    #pragma warning restore EF1002 // Risk of vulnerability to SQL injection. (previously solved through input sanitation)
                    _cache.Add(@params.Search, allProds);
                }
                prodCount = allProds.Count();
                switch (@params.Sort)
                {
                    case "Desc":
                        res = allProds
                                .OrderByDescending(p => p.ListPrice)
                                .Skip((@params.PageIndex - 1) * @params.PageSize)
                                .Take(@params.PageSize)
                                .ToList();
                        break;
                    case "Asc":
                        res = allProds
                                .OrderBy(p => p.ListPrice)
                                .Skip((@params.PageIndex - 1) * @params.PageSize)
                                .Take(@params.PageSize)
                                .ToList();
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
            return (prodCount, res);
        }


        //GET: api/Products/Admin
        //it gets all the products even the ones that are not on sale
        [HttpGet]
        [Route("GetProductsAdmin")]
        [Authorize]
        public async Task<ActionResult<(int, IEnumerable<Product>)>> GetProductsAdmin([FromQuery] ProductSpecParams @params)
        {
            List<Product> res = [];
            int productCount = 0;
            List<ProductDescription> test = [];

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }

            try
            {
                IEnumerable<Product> allProds = [];
                if (@params.Search == null)
                {
                    @params.Search = "";
                }

                // if item is cached, fetch it from cache
                if (_cache.HasItem(@params.Search))
                {
                    allProds = _cache.Get(@params.Search)!;
                }else
                {
                    #pragma warning disable EF1002 // Risk of vulnerability to SQL injection. (solved through input sanitation)
                    allProds = await _context.Products.FromSqlRaw($"SELECT p.* FROM [SalesLT].[Product] AS p WHERE CONTAINS([Name], '\"*{@params.Search.Replace("'", "''").Replace(' ', '*')}*\"') OR FREETEXT([Name], '\"{@params.Search.Replace("'", "''")}\"') UNION SELECT p.* FROM [SalesLT].[Product] AS p INNER JOIN [SalesLT].[ProductModel] AS pm ON p.ProductModelID = pm.ProductModelID INNER JOIN [SalesLT].[ProductModelProductDescription] AS pmpd ON pm.ProductModelID = pmpd.ProductModelID INNER JOIN [SalesLT].[ProductDescription] AS pd ON pmpd.ProductDescriptionID = pd.ProductDescriptionID WHERE CONTAINS(pd.[Description], '\"*{@params.Search.Replace("'", "''").Replace(' ', '*')}*\"') OR FREETEXT(pd.[Description], '\"{@params.Search.Replace("'", "''")}\"')").ToListAsync();
                    #pragma warning restore EF1002 // Risk of vulnerability to SQL injection. (previously solved through input sanitation)
                    _cache.Add(@params.Search, allProds);
                }

                //var allProds = _context.Products
                //                        .Where(prod => ((EF.Functions.FreeText(prod.Name, @params.Search) || prod.ProductModel.ProductModelProductDescriptions.Any(pmpd => (EF.Functions.FreeText(pmpd.ProductDescription.Description, @params.Search) || pmpd.ProductDescription.Description.Contains(@params.Search)))) || (prod.Name.Contains(@params.Search)) && prod.OnSale));
                productCount = allProds.Count();
                switch (@params.Sort)
                {
                    case "Desc":
                        res = allProds
                                .OrderByDescending(p => p.ListPrice)
                                .Skip((@params.PageIndex - 1) * @params.PageSize)
                                .Take(@params.PageSize)
                                .ToList();
                        break;
                    case "Asc":
                        res = allProds
                                .OrderBy(p => p.ListPrice)
                                .Skip((@params.PageIndex - 1) * @params.PageSize)
                                .Take(@params.PageSize)
                                .ToList();
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

        [HttpGet]
        [Route("GetAllProductsAdmin")]
        [Authorize]
        public async Task<ActionResult<(int, IEnumerable<Product>)>> GetAllProductsAdmin([FromQuery] ProductSpecParams @params)
        {
            List<Product> res = [];
            int productCount = 0;
            List<ProductDescription> test = [];

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }

            try
            {
                if (@params.Search == null)
                {
                    @params.Search = "";
                }

                IEnumerable<Product> allProds = [];
                // if item is cached, return it from cache
                if (_cache.HasItem(@params.Search))
                {
                    allProds = _cache.Get(@params.Search)!;
                } else
                {
                    allProds = await _context.Products.FromSql($"SELECT p.* FROM [SalesLT].[Product] AS p UNION SELECT p.* FROM [SalesLT].[Product] AS p INNER JOIN [SalesLT].[ProductModel] AS pm ON p.ProductModelID = pm.ProductModelID INNER JOIN [SalesLT].[ProductModelProductDescription] AS pmpd ON pm.ProductModelID = pmpd.ProductModelID INNER JOIN [SalesLT].[ProductDescription] AS pd ON pmpd.ProductDescriptionID = pd.ProductDescriptionID").ToListAsync();
                    _cache.Add(@params.Search, allProds);
                }

                productCount = allProds.Count();
                switch (@params.Sort)
                {
                    case "Desc":
                        res = allProds
                                .OrderByDescending(p => p.ListPrice)
                                .Skip((@params.PageIndex - 1) * @params.PageSize)
                                .Take(@params.PageSize)
                                .ToList();
                        break;
                    case "Asc":
                        res = allProds
                                .OrderBy(p => p.ListPrice)
                                .Skip((@params.PageIndex - 1) * @params.PageSize)
                                .Take(@params.PageSize)
                                .ToList();
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
            if (product.ProductModelId != null)
            {

                product.ProductModel = await _context.ProductModels.FindAsync(product.ProductModelId);
                if (product.ProductCategoryId != null)
                {
                    product.ProductModel.ProductModelProductDescriptions = await _context.ProductModelProductDescriptions.Where(p => p.ProductModelId == product.ProductModel.ProductModelId).ToListAsync();
                    foreach (var desc in product.ProductModel.ProductModelProductDescriptions)
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
            }
            else
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
                foreach (var prod in await _context.Products.ToListAsync())
                {
                    if (prod.OnSale)
                    {
                        var prediction = RecommendProduct.Predict(new RecommendProduct.ModelInput() { CustomerID = tokenCustomerId, ProductID = prod.ProductId });
                        if (prediction.ProductID != 0F)
                        {
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
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return res;
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


        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<ActionResult<(int, IEnumerable<Product>)>> GetAllProducts([FromQuery] PaginatorParams @params)
        {
            List<Product> res = [];
            int countProducts = 0;
            res = await _context.Products.ToListAsync();
            countProducts = res.Count();
            res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
            switch (@params.Sort)
            {
                case "Desc":
                    res = res.OrderByDescending(el => el.ListPrice).ToList();
                    break;
                case "Asc":
                    res = res.OrderBy(el => el.ListPrice).ToList();
                    break;
            }
            return (countProducts, res);
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
                ProductId = id,
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
                OnSale= productForm.OnSale,
                LargePhoto=productForm.LargePhoto,
                LargePhotoFileName=productForm.LargePhotoFileName


                

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
                OnSale = productForm.OnSale,

                LargePhoto = productForm.LargePhoto,
                LargePhotoFileName = productForm.LargePhotoFileName
            };


            _context.Products.Add(product);
            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
            }
            catch (Exception e)
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
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_context, e);
                return BadRequest();
            }
            return NoContent();
        }
        #endregion

    }
}
