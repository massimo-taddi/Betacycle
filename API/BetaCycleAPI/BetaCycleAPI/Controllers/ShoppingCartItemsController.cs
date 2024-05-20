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
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using NuGet.Versioning;
using BetaCycleAPI.BLogic;

namespace BetaCycleAPI.Controllers
{
    [Route("api/shoppingcart")]
    [ApiController]
    [Authorize]
    public class ShoppingCartItemsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public ShoppingCartItemsController(AdventureWorksLt2019Context awContext, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = awContext;
            _credentialsContext = credentialsContext;
        }

        // GET: api/ShoppingCartItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> GetShoppingCartItems()
        {
            // controllare se il risultato della query e' null, allora crea un nuovo shopping cart vuoto se lo e'. altrimenti ritorna il carrello as described below
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            try
            {
                var shoppingCartId = _awContext.Customers.Find((int)tokenCustomerId).ShoppingCartId;
                return await _awContext.ShoppingCartItems.Where(item => item.ShoppingCartId == shoppingCartId).ToListAsync();
            } catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
        }

        //// GET: api/ShoppingCartItems/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<ShoppingCartItem>> GetShoppingCartItem(int id)
        //{
        //    var shoppingCartItem = await _awContext.ShoppingCartItems.FindAsync(id);

        //    if (shoppingCartItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return shoppingCartItem;
        //}

        // PUT: api/ShoppingCartItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShoppingCartItem(int id, ShoppingCartItem shoppingCartItem)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            if (id != shoppingCartItem.ShoppingCartItemId)
            {
                return BadRequest();
            }

            _awContext.Entry(shoppingCartItem).State = EntityState.Modified;

            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (!ShoppingCartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, e);
                    return BadRequest();
                }
            }

            return NoContent();
        }

        // POST: api/ShoppingCartItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ActionName("PostShoppingCartItemAsync")]
        public async Task<ActionResult<ShoppingCartItem>> PostShoppingCartItem(ShoppingCartItem shoppingCartItem)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   (int)_credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            var customer = await _awContext.Customers.FindAsync(tokenCustomerId);
            ShoppingCart? cart = await _awContext.ShoppingCarts.FindAsync(customer.ShoppingCartId);
            try
            {
                if (cart == null)
                {
                    // create new cart
                    cart = new ShoppingCart
                    {
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                    };
                    _awContext.ShoppingCarts.Add(cart);
                    await _awContext.SaveChangesAsync();
                }
                customer.ShoppingCartId = cart.ShoppingCartId;
                await _awContext.SaveChangesAsync();
                shoppingCartItem.ShoppingCartId = cart.ShoppingCartId;

                _awContext.ShoppingCartItems.Add(shoppingCartItem);
                await _awContext.SaveChangesAsync();
            }catch(Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return CreatedAtAction("PostShoppingCartItemAsync", new { id = shoppingCartItem.ShoppingCartItemId }, shoppingCartItem);
        }

        // DELETE: api/ShoppingCartItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCartItem(int id)
        {
            var shoppingCartItem = await _awContext.ShoppingCartItems.FindAsync(id);
            if (shoppingCartItem == null)
            {
                return NotFound();
            }
            try
            {
                _awContext.ShoppingCartItems.Remove(shoppingCartItem);
                await _awContext.SaveChangesAsync();
            }catch(Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            
            return NoContent();
        }

        private bool ShoppingCartItemExists(int id)
        {
            return _awContext.ShoppingCartItems.Any(e => e.ShoppingCartItemId == id);
        }
    }
}
