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
using BetaCycleAPI.BLogic.ObjectValidator;
using BetaCycleAPI.BLogic;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerReviewsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public CustomerReviewsController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentials)
        {
            _awContext= context;
            _credentialsContext = credentials;
        }

        // GET: api/CustomerReviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReview>>> GetCustomerReviews()
        {
            return await _awContext.CustomerReviews.ToListAsync();
        }

        // GET: api/CustomerReviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerReview>> GetCustomerReview(int id)
        {
            var customerReview = await _awContext.CustomerReviews.FindAsync(id);

            if (customerReview == null)
            {
                return NotFound();
            }

            return customerReview;
        }

        // PUT: api/CustomerReviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerReview(int id, CustomerReview customerReview)
        {
            if (id != customerReview.ReviewId)
            {
                return BadRequest();
            }

            _awContext.Entry(customerReview).State = EntityState.Modified;

            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerReviewExists(id))
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

        // POST: api/CustomerReviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CustomerReview>> PostCustomerReview(CustomerReview customerReview)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            try
            {
                //Creo la recensione
                if (!ModelValidator.ValidateCustomerReview(customerReview))
                    return BadRequest("Campi non corretti");
                _awContext.CustomerReviews.Add(customerReview);
                await _awContext.SaveChangesAsync();
                //Aggiungo al customer l'id della review
                var customer = await _awContext.Customers.FindAsync((int)tokenCustomerId);
                customer.CustomerReviewId = customerReview.ReviewId;
                _awContext.Entry(customer).State = EntityState.Modified;
                await _awContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return CreatedAtAction("GetCustomerReview", new { id = customerReview.ReviewId }, customerReview);
        }

        // DELETE: api/CustomerReviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerReview(int id)
        {
            var customerReview = await _awContext.CustomerReviews.FindAsync(id);
            if (customerReview == null)
            {
                return NotFound();
            }

            _awContext.CustomerReviews.Remove(customerReview);
            await _awContext.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerReviewExists(int id)
        {
            return _awContext.CustomerReviews.Any(e => e.ReviewId == id);
        }
    }
}
