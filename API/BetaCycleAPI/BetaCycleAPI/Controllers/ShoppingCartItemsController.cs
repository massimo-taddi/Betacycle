using BetaCycleAPI.BLogic;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BetaCycleAPI.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/shoppingcart")]
    [ApiController]
    [Authorize]
    public class ShoppingCartItemsController : ControllerBase
    {
        // Contexts
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public ShoppingCartItemsController(AdventureWorksLt2019Context awContext, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = awContext;
            _credentialsContext = credentialsContext;
        }

        #region Private Methods
        private bool ShoppingCartItemExists(int id)
        {
            return _awContext.ShoppingCartItems.Any(e => e.ShoppingCartItemId == id);
        }

        private async Task<bool> isCartEmpty(int shoppingCartId)
        {
            return !(await _awContext.ShoppingCarts.FindAsync(shoppingCartId)).ShoppingCartItems.Any();
        }
        #endregion

        #region Public Methods
        // GET: api/ShoppingCart
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
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
        }

        // GET: api/ShoppingCart/isproductadded
        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("isproductadded")]
        public async Task<ActionResult<bool>> isProductAdded(int productId)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            var cartId = (await _awContext.Customers.FindAsync((int)tokenCustomerId)).ShoppingCartId;
            if (cartId == null)
            {
                return false;
            }

            try
            {
                return await _awContext.ShoppingCartItems.Where(item => item.ProductId == productId && item.ShoppingCartId == cartId).AnyAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("hascart")]
        public async Task<ActionResult<bool>> hasCart()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            try
            {
                var shoppingCartId = (await _awContext.Customers.FindAsync((int)tokenCustomerId)).ShoppingCartId;
                if (shoppingCartId != null)
                {
                    if (await isCartEmpty((int)shoppingCartId))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getcartid")]
        public async Task<ActionResult<int>> GetCartId()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            try
            {
                return (await _awContext.Customers.FindAsync((int)tokenCustomerId)).ShoppingCartId;
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
        }

        // PUT: api/ShoppingCart/5
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

        // POST: api/ShoppingCart
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ActionName("PostShoppingCartItemAsync")]
        [Microsoft.AspNetCore.Mvc.Route("{postFromLocal:int?}")]
        public async Task<ActionResult<ShoppingCartItem>> PostShoppingCartItem([FromBody] ShoppingCartItem shoppingCartItem, bool postFromLocal = false)
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
                    customer.ShoppingCartId = cart.ShoppingCartId;
                }
                if (!(await isCartEmpty(cart.ShoppingCartId)) && postFromLocal)
                {
                    // non aggiungere il prodotto, perche sto mandando dal carrello locale e il carrello su db non e' vuoto
                    return Ok();
                }
                var shoppingCartContainsItem = (await isProductAdded(shoppingCartItem.ProductId)).Value;
                if (shoppingCartContainsItem)
                {
                    var item = await _awContext.ShoppingCartItems.Where(item => item.ProductId == shoppingCartItem.ProductId && item.ShoppingCartId == cart.ShoppingCartId).FirstOrDefaultAsync();
                    item.Quantity += shoppingCartItem.Quantity;
                    await _awContext.SaveChangesAsync();
                }
                else
                {
                    _awContext.ShoppingCartItems.Add(shoppingCartItem);
                }
                shoppingCartItem.ShoppingCartId = cart.ShoppingCartId;

                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return CreatedAtAction("PostShoppingCartItemAsync", new { id = shoppingCartItem.ShoppingCartItemId }, shoppingCartItem);
        }


        // DELETE: api/ShoppingCart/5
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteShoppingCartItem(int productId)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   (int)_credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            var customer = await _awContext.Customers.FindAsync(tokenCustomerId);
            try
            {
                var productsToRemove = await _awContext.ShoppingCartItems.Where(item => item.ProductId == productId && item.ShoppingCartId == customer.ShoppingCartId).ToListAsync();
                foreach (var prod in productsToRemove)
                {
                    _awContext.ShoppingCartItems.Remove(prod);
                }
                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("deletecart/{shoppingCartId}")]
        public async Task<IActionResult> DeleteShoppingCart(int shoppingCartId)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   (int)_credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            var customer = await _awContext.Customers.FindAsync(tokenCustomerId);
            if(customer?.ShoppingCartId != shoppingCartId)
            {
                return BadRequest();
            }
            try
            {
                var cart = await _awContext.ShoppingCarts.FindAsync(customer?.ShoppingCartId);
                if (cart == null)
                {
                    return NotFound();
                }
                foreach (var detail in await _awContext.ShoppingCartItems.Where(det => det.ShoppingCartId == customer.ShoppingCartId).ToListAsync())
                {
                    _awContext.ShoppingCartItems.Remove(detail);
                }
                await _awContext.SaveChangesAsync();
                var cartId = cart.ShoppingCartId;
                _awContext.ShoppingCarts.Remove(cart);
                await _awContext.SaveChangesAsync();
                customer.ShoppingCart = null;
                customer.ShoppingCartId = null;
                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return NoContent();
        }
        #endregion

    }
}
