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

        // GET: api/SalesOrderHeaders
        // get all orders FROM THE CURRENT USER, but if the user is admin then get the full list of orders (ADMINS CANT ORDER!)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrderHeader>>> GetSalesOrderHeaders()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if(token.Claims.First(claim => claim.Type == "role").Value == "admin")
            {
                return await _awContext.SalesOrderHeaders.ToListAsync();
            }

            // TEST VALUES: tokenEmail = "david16@adventure-works.com"; tokenCustomerId = 29847 (o 609);
            // E' stato testato con dei valori di prova e funziona
            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).Last().CustomerId;
            return await _awContext.SalesOrderHeaders.Where(order => order.CustomerId == tokenCustomerId).ToListAsync();
        }

        // GET: api/SalesOrderHeaders/5
        // get an order with a provided ID
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrderHeader>> GetSalesOrderHeader(int id)
        {
            var salesOrderHeader = await _awContext.SalesOrderHeaders.FindAsync(id);

            if (salesOrderHeader == null)
            {
                return NotFound();
            }

            return salesOrderHeader;
        }

        // PUT: api/SalesOrderHeaders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderHeader(int id, SalesOrderHeader salesOrderHeader)
        {
            if (id != salesOrderHeader.SalesOrderId)
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

        // POST: api/SalesOrderHeaders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            _awContext.SalesOrderHeaders.Add(salesOrderHeader);
            await _awContext.SaveChangesAsync();

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        // DELETE: api/SalesOrderHeaders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderHeader(int id)
        {
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
