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
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams @params)
        {
            List<Product> res = [];
            switch (@params.Sort)
            {
                case "priceDesc":
                    res = await (from product in _context.Products 
                                 join pmpd in _context.ProductModelProductDescriptions
                                 on product.ProductModelId equals pmpd.ProductModelId
                                 join descr in _context.ProductDescriptions on pmpd.ProductDescriptionId equals descr.ProductDescriptionId
                                 where product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)
                                 select product).Skip((@params.PageIndex - 1) * @params.PageSize)
                                                .Take(@params.PageSize)
                                                .OrderByDescending(p => p.ListPrice)
                                                .ToListAsync();
                    //res = await _context.Products
                    //                    .Where(prod => prod.Name.Contains(@params.Search))
                    //                    .OrderBy(prod => prod.ListPrice)
                    //                    .Skip((@params.PageIndex - 1) * @params.PageSize)
                    //                    .Take(@params.PageSize).ToListAsync();
                    break;
                case "priceAsc":
                    res = await (from product in _context.Products
                                 join pmpd in _context.ProductModelProductDescriptions
                                 on product.ProductModelId equals pmpd.ProductModelId
                                 join descr in _context.ProductDescriptions on pmpd.ProductDescriptionId equals descr.ProductDescriptionId
                                 where product.Name.Contains(@params.Search) || descr.Description.Contains(@params.Search)
                                 select product).OrderBy(p => p.ListPrice).Skip((@params.PageIndex - 1) * @params.PageSize)
                                                                          .Take(@params.PageSize)
                                                                          .OrderBy(p => p.ListPrice)
                                                                          .ToListAsync();
                    break;
                default:
                    return BadRequest();
            }

            return res;
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

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (id != product.ProductId || token.Claims.First(claim => claim.Type == "role").Value == "admin")
            {
                return BadRequest();
            }

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
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest();
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
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest();
            var product = await _context.Products.FindAsync(id);
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
