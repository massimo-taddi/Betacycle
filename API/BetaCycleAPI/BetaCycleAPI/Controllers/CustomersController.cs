using BetaCycleAPI.BLogic;
using BetaCycleAPI.BLogic.ObjectValidator;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        // Contexts
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public CustomersController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
        }
        #region Private Methods
        private bool CustomerExists(int id)
        {
            return _awContext.Customers.Any(e => e.CustomerId == id);
        }
        private void writeToAiFile(int customerID)
        {
            string toAppend = string.Empty;
            foreach (var prod in _awContext.Products)
            {
                toAppend += $"{customerID};{prod.ProductId};0\n";
            }
            System.IO.File.AppendAllText("Data/aiTrainingData.csv", toAppend);
        }


        private async Task<IActionResult> deleteCustomerLogic(int id) //it could be tokenCustomerID or id given by an admin
        {
            try
            {
                bool found = false;
                var customer = await _awContext.Customers.FindAsync((int)id);
                _awContext.Entry(customer).State = EntityState.Detached;
                var credentials = await _credentialsContext.Credentials.FindAsync((long)id);
                if (credentials != null) //check if the user is migrated
                {
                    _credentialsContext.Entry(credentials).State = EntityState.Detached;
                    found = true;
                }
                //Get customer's related ids
                var addressIds = await(from custom in _awContext.Customers
                                       join custAddr in _awContext.CustomerAddresses on custom.CustomerId equals custAddr.CustomerId
                                       join address in _awContext.Addresses on custAddr.AddressId equals address.AddressId
                                       where custom.CustomerId == (int)id
                                       select address.AddressId).ToListAsync();
                if (addressIds.Any()) //it has addresses
                {
                    var headers = await(from custom in _awContext.Customers
                                        join head in _awContext.SalesOrderHeaders on custom.CustomerId equals head.CustomerId
                                        where custom.CustomerId == (int)id
                                        select head).ToListAsync();
                    if (headers.Any()) //It has past orders, so need to set null the following fields to keep RI
                    {
                        headers.ForEach(head =>
                        {
                            head.ShipToAddressId = null;
                            head.BillToAddressId = null;
                            head.CustomerId = null;
                            _awContext.Entry(head).State = EntityState.Modified;
                        });
                        await _awContext.SaveChangesAsync();
                    }
                    //Delete customer Addresses records
                    IQueryable<CustomerAddress> custAddresses = from custom in _awContext.Customers
                                                                join custAdd in _awContext.CustomerAddresses
                                                                on custom.CustomerId equals custAdd.CustomerId
                                                                where custom.CustomerId == (int)id
                                                                select custAdd;
                    _awContext.RemoveRange(custAddresses);
                    await _awContext.SaveChangesAsync();

                    //Delete addresses records 
                    addressIds.ForEach(add =>
                    {
                        _awContext.Addresses.Remove(new Address() { AddressId = (int)add });
                    });
                    await _awContext.SaveChangesAsync();
                }
                //it has no address registered so no past orders
                if (customer != null)
                {
                    _awContext.Customers.Remove(customer!);
                    if (found)
                    {
                        _credentialsContext.Credentials.Remove(credentials!);
                        await _credentialsContext.SaveChangesAsync();
                    }
                    await _awContext.SaveChangesAsync();
                }
                else return BadRequest();
            }
            catch (Exception ex)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, ex);
                return BadRequest();
            }
            return NoContent();
        }


        #endregion

        #region Public Methods
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
            try
            {
                if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                {
                    switch (@params.Sort)
                    {
                        case "Desc":
                            if (@params.Search == null)
                            {
                                @params.Search = "";
                            }
                            // Returns n customers 
                            res = await (from customer in _awContext.Customers
                                         where customer.FirstName.Contains(@params.Search) || customer.CompanyName.Contains(@params.Search)

                                         select customer)
                                         .OrderBy(x => x.FirstName).ToListAsync();
                            customerCount = res.Count();
                            res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();

                            break;
                        case "Asc":
                            // Returns n customers 
                            res = await (from customer in _awContext.Customers
                                         where customer.FirstName.Contains(@params.Search) || customer.CompanyName.Contains(@params.Search)

                                         select customer)
                                         .OrderBy(x => x.FirstName).ToListAsync();
                            customerCount = res.Count();
                            res = res.Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize).ToList();
                            break;
                    }

                    return (customerCount, res);

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
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return (customerCount, res);
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
            try
            {
                customer.CustomerAddresses = await _awContext.CustomerAddresses.Where(ad => ad.CustomerId == id).ToListAsync();
                foreach (var item in customer.CustomerAddresses)
                {
                    item.Address = await _awContext.Addresses.FindAsync(item.AddressId);
                }
                if (customer == null)
                    return NotFound();
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
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
            catch (Exception e)
            {
                if (!CustomerExists(id))
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
            try
            {
                if (IsMailTaken(customer.EmailAddress)) return BadRequest("Mail is already taken");
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
                writeToAiFile(customer.CustomerId);
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        [HttpGet]
        [Route("isMailTaken/{mail}")]
         // GET: api/Customers/isMailTaken
        public bool IsMailTaken(string mail)
        {
            if (_credentialsContext.Credentials.Any(c => c.Email.ToLower() == mail.ToLower()))
                return true;
            else
                return false;
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCustomer(int id=0)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
            { //Admin case
                try
                {
                    await deleteCustomerLogic(id);
                }catch(Exception ex)
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, ex);
                    return BadRequest();
                }
            }
            else
            { // Customer case
                try
                {
                    await deleteCustomerLogic((int)tokenCustomerId);
                }
                catch (Exception ex)
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, ex);
                    return BadRequest();
                }
            }
            return NoContent();
        }

        #endregion
    }
}
