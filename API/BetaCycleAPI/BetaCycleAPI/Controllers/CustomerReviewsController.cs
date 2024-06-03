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
using Microsoft.ML;
using System.Runtime.CompilerServices;

namespace BetaCycleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<ReviewDataForm>>> GetCustomerReviews()
        {
            List<ReviewDataForm> res = [];
            try
            {
                var myReviews = await _awContext.CustomerReviews.ToListAsync();
                Random numGen = new Random();
                int[] generatedNums = [];
                int generated;
                for (int i = 0; i < 5; i++)
                {
                    do
                    {
                        generated = (int)numGen.Next(myReviews.Count);
                    } while (generatedNums.Contains(generated));
                    generatedNums = generatedNums.Append(generated).ToArray();
                    var rev = myReviews[generated];
                    if (rev.Rating >= 3)
                    {
                        var cust = await _awContext.Customers.Where(c => c.CustomerReviewId == rev.ReviewId).FirstOrDefaultAsync();

                        res.Add(new ReviewDataForm()
                        {
                            ReviewId = rev.ReviewId,
                            BodyDescription = rev.BodyDescription,
                            Rating = rev.Rating,
                            ReviewDate = rev.ReviewDate,
                            ModifiedDate = rev.ModifiedDate,
                            CustomerName = cust == null ? "Anonymous" : cust.FirstName
                        });
                    }
                    else i--;
                }
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return res.OrderByDescending(rev => rev.Rating).ToList();
        }

        // GET: api/CustomerReviews/5

        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public async Task<ActionResult<CustomerReview>> PostCustomerReview(CustomerReview customerReview)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(await HttpContext.GetTokenAsync("access_token"));

            var tokenEmail = token.Claims.First(claim => claim.Type == "unique_name").Value;
            var tokenCustomerId = _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).IsNullOrEmpty() ?
                                   _awContext.Customers.Where(customer => customer.EmailAddress == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId :
                                   _credentialsContext.Credentials.Where(customer => customer.Email == tokenEmail).OrderBy(c => c.CustomerId).Last().CustomerId;

            var customerCheck = await _awContext.Customers.FindAsync((int)tokenCustomerId);
            _awContext.Entry(customerCheck).State = EntityState.Detached;

            if (customerCheck.CustomerReviewId != null)
            {
                try
                {
                    customerReview.Rating = (byte)Convert.ToInt32(this.GetReviewScore(customerReview.BodyDescription));
                    var id = customerCheck!.CustomerReviewId;
                    var oldReview = await _awContext.CustomerReviews.FindAsync((int)id);
                    _awContext.Entry(oldReview).State = EntityState.Detached;
                    customerReview.ReviewDate = oldReview.ReviewDate;
                    customerReview.ReviewId = (int)id;
                    await PutCustomerReview((int)id, customerReview);
                }catch(Exception ex)
                {
                    await DBErrorLogger.WriteExceptionLog(_awContext, ex);
                    return BadRequest();
                }
            }
            else
            {
                try
                {
                    //Creo la recensione
                    if (!ModelValidator.ValidateCustomerReview(customerReview))
                        return BadRequest("Campi non corretti");
                    customerReview.Rating = (byte)Convert.ToInt32(this.GetReviewScore(customerReview.BodyDescription));
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
            }
                return CreatedAtAction("GetCustomerReview", new { id = customerReview.ReviewId }, customerReview);
        }


        //


        [HttpPost]
        [Route("GetReviewScore")]
        public float GetReviewScore([FromBody] string text)
        {
            float result = 0.0f;

            WebsiteReviewRating.ModelOutput predictions = WebsiteReviewRating.Predict(new WebsiteReviewRating.ModelInput { ReviewText = text });

            for (int i = 1; i < 6; i++)
            {
                result += predictions.Score[this.scoreIndex(i)] * i;
            }

            return result;
        }

        // 1 stella - pos 4
        // 2 stelle - pos 1
        // 3 stelle - pos 3
        // 4 stelle - pos 2
        // 5 stelle - pos 0
        // 2, 4, 3, 1, 0
        private int scoreIndex(int i)
        {
            switch (i)
            {
                case 1:
                    return 4;
                case 2:
                    return 1;
                case 3:
                    return 3;
                case 4:
                    return 2;
                case 5:
                    return 0;
                default:
                    return 0;
            }
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
