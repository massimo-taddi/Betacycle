using BetaCycleAPI.BLogic;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

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
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                headers = await _awContext.SalesOrderHeaders.ToListAsync();
            else
            {
                var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
                var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                       _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                       _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
                headers = await _awContext.SalesOrderHeaders.Where(order => order.CustomerId == tokenCustomerId).ToListAsync();
            }

            try
            {
                foreach (var header in headers)
                {
                    header.SalesOrderDetails = await _awContext.SalesOrderDetails.Where(detail => detail.SalesOrderId == header.SalesOrderId).ToListAsync();
                    foreach (var detail in header.SalesOrderDetails)
                    {
                        detail.Product = await _awContext.Products.FindAsync(detail.ProductId);
                    }
                    header.ShipToAddress = await _awContext.Addresses.Where(address => address.AddressId == header.ShipToAddressId).FirstAsync();
                }
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
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
            catch (DbUpdateConcurrencyException e)
            {
                if (!SalesOrderHeaderExists(id))
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

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<SalesOrderHeader>> PostSalesOrderHeader(SalesOrderHeader salesOrderHeader)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            salesOrderHeader.CustomerId = (int)tokenCustomerId;
            salesOrderHeader.OrderDate = DateTime.Now;
            salesOrderHeader.DueDate = DateTime.Now.AddDays(7);
            salesOrderHeader.Status = 1;
            salesOrderHeader.OnlineOrderFlag = true;
            salesOrderHeader.Rowguid = Guid.NewGuid();
            salesOrderHeader.ModifiedDate = DateTime.Now;

            salesOrderHeader.SubTotal = salesOrderHeader.SalesOrderDetails.Sum(detail => detail.LineTotal);
            salesOrderHeader.Freight = await calculateFreightCost(salesOrderHeader.SalesOrderDetails, salesOrderHeader.ShipMethod);
            if (salesOrderHeader.Freight == 0.0m) return BadRequest(); // if freight cost calculation fails, return 400
            // TaxAmt is passed in percentage, so it has to be converted to an absolute value first
            salesOrderHeader.TaxAmt = salesOrderHeader.SubTotal * salesOrderHeader.TaxAmt / 100;
            salesOrderHeader.TotalDue = salesOrderHeader.SubTotal + salesOrderHeader.Freight + salesOrderHeader.TaxAmt;

            _awContext.SalesOrderHeaders.Add(salesOrderHeader);
            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }

        private async Task<decimal> calculateFreightCost(ICollection<SalesOrderDetail> details, string shipMethod)
        {
            decimal res = 0;
            decimal costPerKg = 0.0m;
            try
            {
                using (StreamReader r = new StreamReader(Path.GetFullPath("Data\\ShippingCostsPerKg.json")))
                {
                    string json = r.ReadToEnd();
                    var costsPerKg = JsonConvert.DeserializeObject<List<KeyValuePair<string, decimal>>>(json);
                    costPerKg = costsPerKg.First(pair => pair.Key == shipMethod).Value;
                }
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return 0.0m;
            }

            foreach (var detail in details)
            {
                if (detail.Product.Weight != null)
                {
                    res += (decimal)detail.Product.Weight * costPerKg;
                } // if the product has no weight, the weight of the parcel is fixed at 1kg
                else
                {
                    res += costPerKg;
                }

            }
            return res;
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

            try
            {
                _awContext.SalesOrderHeaders.Remove(salesOrderHeader);
                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return NoContent();
        }

        private bool SalesOrderHeaderExists(int id)
        {
            return _awContext.SalesOrderHeaders.Any(e => e.SalesOrderId == id);
        }
    }
}
