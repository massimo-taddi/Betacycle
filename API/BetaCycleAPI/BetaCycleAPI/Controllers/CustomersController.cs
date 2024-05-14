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
using BetaCycleAPI.BLogic.ObjectValidator;
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
        [Authorize]
        public async Task<ActionResult<(int, IEnumerable<Customer>)>> GetCustomers([FromQuery] ProductSpecParams @params)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            int customerCount = 0;
            List<Customer> res = [];
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
            {
                switch (@params.Sort)
                {
                    case "Desc":
                        //ritornare n customer 
                        res = await (from customer in _awContext.Customers
                                     where customer.FirstName.Contains(@params.Search) || customer.CompanyName.Contains(@params.Search) || customer != null
                                     select customer)
                                     .OrderBy(x => x.FirstName).ToListAsync();
                        customerCount = res.Count();  
                            
                        break;
                    case "Asc":
                        //ritornare n customer 
                        res = await (from customer in _awContext.Customers
                                     where customer.FirstName.Contains(@params.Search) || customer.CompanyName.Contains(@params.Search) || customer!=null
                                     select customer)
                                     .OrderBy(x => x.FirstName).ToListAsync();
                        customerCount = res.Count();
                        break;
                }             
                res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                return (customerCount,res);
                
            }
            else
            {
                var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
                var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                       _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                       _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
                res = await _awContext.Customers.Where(customer => customer.CustomerId == tokenCustomerId).ToListAsync();
            }
            foreach (var customer in res)
            {
                customer.CustomerAddresses = await _awContext.CustomerAddresses.Where(ca => ca.CustomerId == customer.CustomerId).ToListAsync();
            }
            
            return (customerCount,res);
        }


        // GET: api/Customers/5
        /// <summary>
        /// Get a <c>Customer</c> object with the specified id
        /// only allowed for admins
        /// </summary>
        /// <param name="id">The id of the <c>Customer</c> to find</param>
        /// <returns>The customer with the specified id</returns>
        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }
            if (!ModelValidator.ValidateCustomer(customer))
                return BadRequest("Campi non validi");

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
        /// Given a SignUpForm object, posts it to the DB
        /// </summary>
        /// <param name="toInsert">The SignUpForm to insert</param>
        /// <returns>A Task<ActionResult<Customer>> representing the posted customer</returns>
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer([FromBody] SignUpForm toInsert)
        {
            Customer customer = new Customer
            {
                Title = toInsert.Title,
                FirstName = toInsert.FirstName,
                MiddleName = toInsert.MiddleName,
                LastName = toInsert.LastName,
                Suffix = toInsert.Suffix,
                CompanyName = toInsert.CompanyName,
                SalesPerson = toInsert.SalesPerson,
                EmailAddress = toInsert.EmailAddress,
                Phone = toInsert.Phone,
                IsMigrated = toInsert.IsMigrated,
                // CustomerAddresses = toInsert.CustomerAddresses
            };
            // customer.PasswordHash is NOT supposed to be hashed at this point
            KeyValuePair<string, string> pwData = EncryptData.CypherData.SaltEncryp(toInsert.Password);
            // this is where it gets hashed ^
            customer.PasswordHash = pwData.Key;
            Console.WriteLine(pwData.Value);
            customer.PasswordSalt = pwData.Value;
            if (!ModelValidator.ValidateCustomer(customer))
                return BadRequest("Campi non validi");
            // the changes are only written on _awContext since the data is automatically migrated to the Credentials DB
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
