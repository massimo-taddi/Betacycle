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
using System.Net;
using Humanizer;
using Microsoft.SqlServer.Server;
using System.Net.Sockets;
using BetaCycleAPI.BLogic.ObjectValidator;
using BetaCycleAPI.BLogic;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;

        public AddressesController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
        }

        // GET: api/Addresses
        // for a customer, gets all of their addresses. for an admin, gets all the addresses on the db
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            List<Address> res = [];
          

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            try
            {
                if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                    res = await _awContext.Addresses.ToListAsync();
                else
                {
                    res = await _awContext.Addresses.Where(add => _awContext.CustomerAddresses.Where(ca => ca.AddressId == add.AddressId).First().CustomerId == tokenCustomerId).ToListAsync();
                }
                foreach (var address in res)
                {
                    List<CustomerAddress> custAdd = await _awContext.CustomerAddresses.Where(ca => ca.AddressId == address.AddressId && ca.CustomerId == tokenCustomerId).ToListAsync();
                    address.CustomerAddresses = custAdd;
                }
            } catch(Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            return res;
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _awContext.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, AddressFormData formData)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            Address putAddress = new Address();
            CustomerAddress putCustAdd = new CustomerAddress();

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            Address getAddressForGuid = await _awContext.Addresses.FindAsync(id);
            _awContext.Entry(getAddressForGuid).State = EntityState.Detached;
            CustomerAddress getCustAddForGuid = await _awContext.CustomerAddresses.Where(ca => ca.AddressId == id && ca.CustomerId == tokenCustomerId).FirstAsync();
            _awContext.Entry(getCustAddForGuid).State = EntityState.Detached;

            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest("Account admin rilevato");
            else
            {
                try
                {
                    var modDate = DateTime.Now;
                    putAddress = new Address()
                    {
                        AddressId = id,
                        AddressLine1 = formData.AddressLine1,
                        AddressLine2 = formData.AddressLine2,
                        City = formData.City,
                        StateProvince = formData.StateProvince,
                        CountryRegion = formData.CountryRegion,
                        PostalCode = formData.PostalCode,
                        Rowguid = getAddressForGuid.Rowguid,
                        ModifiedDate = modDate
                    };
                    if (!ModelValidator.ValidateAddress(putAddress))
                        return BadRequest("Campi non validi");
                    _awContext.Entry(putAddress).State = EntityState.Modified;
                    await _awContext.SaveChangesAsync();
                    putCustAdd = new CustomerAddress()
                    {
                        CustomerId = (int)tokenCustomerId,
                        AddressId = putAddress.AddressId,
                        AddressType = formData.AddressType,
                        Rowguid = getCustAddForGuid.Rowguid,
                        ModifiedDate = modDate
                    };
                    _awContext.Entry(putCustAdd).State = EntityState.Modified;
                    await _awContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, e);
                    return BadRequest();
                }
            }
            return Ok();
        }

        // POST: api/Addresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress([FromBody] AddressFormData formData)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            Address address = null;
            CustomerAddress custAdd = null;
            List<Address> res = [];
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest("Account admin rilevato");
            else
            {
                try
                {
                    var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
                    var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                           _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                           _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
                    //Save address in db
                    var modDate = DateTime.Now;
                    address = new Address()
                    {
                        AddressLine1 = formData.AddressLine1,
                        AddressLine2 = formData.AddressLine2,
                        City = formData.City,
                        StateProvince = formData.StateProvince,
                        CountryRegion = formData.CountryRegion,
                        PostalCode = formData.PostalCode,
                        ModifiedDate = modDate
                    };
                    if (!ModelValidator.ValidateAddress(address))
                        return BadRequest("Campi non validi");
                    _awContext.Addresses.Add(address);
                    await _awContext.SaveChangesAsync();
                    custAdd = new CustomerAddress()
                    {
                        CustomerId = (int)tokenCustomerId,
                        AddressId = address.AddressId,
                        AddressType = formData.AddressType,
                        ModifiedDate = modDate
                    };
                    _awContext.CustomerAddresses.Add(custAdd);
                    await _awContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, e);
                    return BadRequest();
                }
            }
            return Ok();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));
            Address putAddress = new Address();
            CustomerAddress putCustAdd = new CustomerAddress();

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
            CustomerAddress custAdd = await _awContext.CustomerAddresses.Where(ca => ca.AddressId == id && ca.CustomerId == tokenCustomerId).FirstAsync();
            _awContext.Entry(custAdd).State = EntityState.Detached;
            Address addr = new Address() { AddressId = id };
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                return BadRequest("Account admin rilevato");
            else
            {
                try
                {
                    _awContext.CustomerAddresses.Remove(custAdd);
                    await _awContext.SaveChangesAsync();
                    _awContext.Addresses.Remove(addr);
                    await _awContext.SaveChangesAsync();
                }catch(Exception e)
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, e);
                    return BadRequest();
                }
            }
            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _awContext.Addresses.Any(e => e.AddressId == id);
        }
    }
}
