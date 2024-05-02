using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using BetaCycleAPI.Models.ModelsCredentials;
using EncryptData;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Authorization;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public CustomersController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
        }

        // GET: api/Customers
        /// <summary>
        /// Administrators get a list of all customer information, while customers get a list with a single item containing their information
        /// </summary>
        /// <returns>admins: A list of all the customers found, customers: a list with a single item containing their information</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return await _awContext.Customers.ToListAsync();
            else
            {
                var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
                var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                       _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                       _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
                return await _awContext.Customers.Where(customer => customer.CustomerId == tokenCustomerId).ToListAsync();
            }
        }

        // GET: api/Customers/5
        /// <summary>
        /// Get a <c>Customer</c> object with the specified id
        /// only allowed for admins
        /// </summary>
        /// <param name="id">The id of the <c>Customer</c> to find</param>
        /// <returns>The customer with the specified id</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest();
            var customer = await _awContext.Customers.FindAsync(id);
            List<CustomerAddress> addresses = [];
            customer.CustomerAddresses = await _awContext.CustomerAddresses.Where(ad => ad.CustomerId == id).ToListAsync();
            foreach (var item in customer.CustomerAddresses)
            {
                item.Address = await _awContext.Addresses.FindAsync(item.AddressId);
            }
            if (customer == null)
                return NotFound();

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Updates a customer record given a record and the id of the customer
        /// </summary>
        /// <param name="id">The id of the customer</param>
        /// <param name="customer">The customer record representing the updated information</param>
        /// <returns>The response, which depends on the DB response</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _awContext.Entry(customer).State = EntityState.Modified;

            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// Given a Customer object, posts it to the DB
        /// </summary>
        /// <param name="customer">The Customer object, from which fields CustomerId, SaltHash and rowguid are ignored</param>
        /// <returns>A Task<ActionResult<Customer>> representing the posted customer</returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            if(!customer.Validate()) return BadRequest();
            // customer.PasswordHash is NOT supposed to be hashed at this point
            KeyValuePair<string, string> pwData = EncryptData.CypherData.SaltEncryp(customer.PasswordHash);
            // this is where it gets hashed ^
            customer.PasswordHash = pwData.Key;
            Console.WriteLine(pwData.Value);
            customer.PasswordSalt = pwData.Value;
            _awContext.Customers.Add(customer);
            await _awContext.SaveChangesAsync();
            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        //[HttpPost]
        //public async Task<ActionResult<Boolean>> PostLogin(string email, string password)
        //{
        //    bool isValid = false;
        //    var myCredentials = await _credentials.Credentials.FindAsync(email); //da verificare

        //    return CreatedAtAction("Login", isValid);
        //}

        // DELETE: api/Customers/5
        /// <summary>
        /// Deletes a customer record from the DB
        /// </summary>
        /// <param name="id">The id of the customer to delete</param>
        /// <returns>A response indicating whether the record was deleted or not</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _awContext.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _awContext.Customers.Remove(customer);
            await _awContext.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _awContext.Customers.Any(e => e.CustomerId == id);
        }
    }
}
