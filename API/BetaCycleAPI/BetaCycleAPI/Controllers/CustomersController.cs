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
using BetaCycleAPI.BLogic.Authentication.Basic;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public CustomersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        [BasicAuthorizationAttributes]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            List<CustomerAddress> addresses = [];
            customer.CustomerAddresses = await _context.CustomerAddresses.Where(ad => ad.CustomerId == id).ToListAsync();
            foreach (var item in customer.CustomerAddresses)
            {
                item.Address = await _context.Addresses.FindAsync(item.AddressId);
            }

            if (customer == null)
                return NotFound();

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
