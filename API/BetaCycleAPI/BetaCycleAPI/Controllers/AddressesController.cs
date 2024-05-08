﻿using System;
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
            if (token.Claims.First(claim => claim.Type == "role").Value == "admin")
                res = await _awContext.Addresses.ToListAsync();
            else
            {
                var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
                var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                       _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                       _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;
                res = await _awContext.Addresses.Where(add => _awContext.CustomerAddresses.Where(ca => ca.AddressId == add.AddressId).First().CustomerId == tokenCustomerId).ToListAsync();
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
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressId)
            {
                return BadRequest();
            }

            _awContext.Entry(address).State = EntityState.Modified;

            try
            {
                await _awContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return CreatedAtAction("GetAddress", new { id = address.AddressId }, address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _awContext.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _awContext.Addresses.Remove(address);
            await _awContext.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _awContext.Addresses.Any(e => e.AddressId == id);
        }
    }
}
