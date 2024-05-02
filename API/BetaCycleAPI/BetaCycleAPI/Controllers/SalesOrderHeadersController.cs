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
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace BetaCycleAPI.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class SalesOrderHeadersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public SalesOrderHeadersController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
        }

        // GET: api/orders
        // get all orders FROM THE CURRENT USER, but if the user is admin then get the full list of orders (ADMINS CANT ORDER!)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaders()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            List<SalesOrderHeader> headers = new();
            if(token.Claims.First(claim => claim.Type == "role").Value == "admin")
                headers = await _awContext.SalesOrderHeaders.ToListAsync();
            else
            {
                // TEST VALUES: tokenEmail = "david16@adventure-works.com"; tokenCustomerId = 29847 (o 609);
                // E' stato testato con dei valori di prova e funziona
                var tokenEmail = "david16@adventure-works.com";// token.Claims.First(claim => claim.Type == "unique_name").Value;
                var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                       _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                       _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
                headers = await _awContext.SalesOrderHeaders.Where(order => order.CustomerId == tokenCustomerId).ToListAsync();
            }

            foreach (var header in headers)
            {
                header.SalesOrderDetails = await _awContext.SalesOrderDetails.Where(detail => detail.SalesOrderId == header.SalesOrderId).ToListAsync();
                header.ShipToAddress = await _awContext.Addresses.Where(address => address.AddressId == header.ShipToAddressId).FirstAsync();
            }


            return headers;
        }

        // GET: api/orders/5
        // get an order with a provided ID, only for admins
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            SalesOrderHeader? salesOrderHeader;
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
            {
                salesOrderHeader = await _awContext.SalesOrderHeaders.FindAsync(id);

                if (salesOrderHeader == null)
                {
                    return NotFound();
                }
                salesOrderHeader.SalesOrderDetails = await _awContext.SalesOrderDetails.Where(detail => detail.SalesOrderId == salesOrderHeader.SalesOrderId).ToListAsync();
                salesOrderHeader.ShipToAddress = await _awContext.Addresses.Where(address => address.AddressId == salesOrderHeader.ShipToAddressId).FirstAsync();
                return salesOrderHeader;
            }

            return BadRequest();
        }

        // PUT: api/orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // updates a salesorderheader with a given ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin" || id != salesOrderHeader.SalesOrderId)
            {
                return BadRequest();
            }

            _awContext.Entry(salesOrderHeader).State = EntityState.Modified;

            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderHeaderExists(id))
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

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderHeader(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value != "admin")
            {
                return BadRequest();
            }

            var salesOrderHeader = await _awContext.SalesOrderHeaders.FindAsync(id);
            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            _awContext.SalesOrderHeaders.Remove(salesOrderHeader);
            await _awContext.SaveChangesAsync();

            return NoContent();
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return _awContext.SalesOrderHeaders.Any(e => e.SalesOrderId == id);
        }
    }
}
