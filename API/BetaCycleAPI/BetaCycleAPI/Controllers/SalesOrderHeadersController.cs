using BetaCycleAPI.BLogic;
using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace BetaCycleAPI.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class SalesOrderHeadersController : ControllerBase
    {
        // Contexts
        private readonly AdventureWorksLt2019Context _awContext;
        private readonly AdventureWorks2019CredentialsContext _credentialsContext;
        private readonly string _siteUrl;
        private readonly JwtSettings _jwtSettings;

        public SalesOrderHeadersController(AdventureWorksLt2019Context context, AdventureWorks2019CredentialsContext credentialsContext, string siteUrl, JwtSettings jwtSettings)
        {
            _awContext = context;
            _credentialsContext = credentialsContext;
            _siteUrl = siteUrl;
            _jwtSettings = jwtSettings;
        }

        #region Private Methods
        private bool SalesOrderHeaderExists(int id)
        {
            return _awContext.SalesOrderHeaders.Any(e => e.SalesOrderId == id);
        }

        private async Task<ActionResult<bool>> SendConfirmationEmail(SalesOrderHeader header, string email, string token)
        {
            MailMessage mail = new();
            SmtpClient smtpClient = new("smtp.gmail.com");

            mail.From = new MailAddress("beta89256464@gmail.com");
            mail.To.Add(email);
            mail.Subject = "Your Betacycle Order Confirmation";
            mail.IsBodyHtml = true;

            string tBody = "<tbody>";
            foreach (var detail in header.SalesOrderDetails)
            {
                var attachment = new Attachment(new MemoryStream(detail.Product?.ThumbNailPhoto), detail.Product.ThumbnailPhotoFileName, "image/gif");
                mail.Attachments.Add(attachment);
                attachment.ContentId = detail.Product?.ProductId.ToString();
                attachment.ContentDisposition.Inline = true;
                string prodImg = "<img style=\"width: 60px;\" src=\"cid:" + attachment.ContentId + "\" alt=\"...\">";
                tBody += "<tr><td>" + prodImg + "</td><td>" + detail.Product?.Name + "</td><td>" + detail.OrderQty + "</td><td>$" + Math.Round(detail.LineTotal, 2) + "</td>" + "</tr>";
            }
            tBody += "</tbody>";

            //sitereview => new component name
            mail.Body = "<!doctypehtml><meta charset=UTF-8><title>Order Confirmation</title><style>body{font-family:Arial,sans-serif;background-color:#f2f2f2;margin:0;padding:20px}h1{color:#333}table{width:100%;border-collapse:collapse;margin-bottom:20px}td,th{padding:10px;text-align:left;border-bottom:1px solid #ccc}th{font-weight:700}ul{list-style-type:none;padding:0}li{margin-bottom:5px}p{margin-bottom:10px}</style><h1>Order Confirmation</h1><p>Thank you for your order! Here are the details:<table><tr><th>Order Number:<td>" + header.SalesOrderNumber + "<tr><th>Order Date:<td>" + header.OrderDate + "<tr><th>Products:<td><table><thead><tr><th>Image<th>Name<th>Quantity<th>Total<th></thead>" + tBody + "</table></table><b>Order Total: $" + Math.Round(header.TotalDue, 2) + "</b><p>If you have any questions, please contact our customer support.</p><p>Thank you for shopping with us!</p><button style=\"margin-top:20px;padding: 15px 25px;color: white;background-color: #687995;border: 1px solid #687995;border-radius: 10px;\"><a href=\"http://" + _siteUrl + $":4200/sitereview?token={token}\">Review your experience with us!</a></button></body></html>";

            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential("beta89256464@gmail.com", "ooriltjjyrjekmvi");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return true;
        }
        #endregion

        #region Public Methods
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
                    //header.SalesOrderDetails = await _awContext.SalesOrderDetails.Where(detail => detail.SalesOrderId == header.SalesOrderId).ToListAsync();
                    //header.ShipToAddress = await _awContext.Addresses.Where(address => address.AddressId == header.ShipToAddressId).FirstAsync();

                }
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return headers;
        }
        


        [HttpGet]
        [Route("details/{headerId}")]
        public async Task<ActionResult<IEnumerable<SalesOrderDetail>>> GetSalesOrderDetails(int headerId)
        {
            List<SalesOrderDetail> results = [];
            try
            {
                //prendo tutti i details del mio header
                var myDetails = await _awContext.SalesOrderDetails.Where(det => det.SalesOrderId == headerId).ToListAsync();
                foreach (var detail in myDetails)
                {
                    detail.Product = await _awContext.Products.FindAsync(detail.ProductId);
                }
                results = myDetails;
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }
            return results;
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
            salesOrderHeader.Status = 1;
            salesOrderHeader.OnlineOrderFlag = true;
            salesOrderHeader.Rowguid = Guid.NewGuid();
            salesOrderHeader.ModifiedDate = DateTime.Now;
            switch(salesOrderHeader.ShipMethod)
            {
                case "UPS":
                    salesOrderHeader.DueDate = (salesOrderHeader.OrderDate.AddDays(7));
                    break;
                case "FedEx":
                    salesOrderHeader.DueDate = (salesOrderHeader.OrderDate.AddDays(10));
                    break;
                case "USPS":
                    salesOrderHeader.DueDate = (salesOrderHeader.OrderDate.AddDays(12));
                    break;
                case "DHL":
                    salesOrderHeader.DueDate = (salesOrderHeader.OrderDate.AddDays(5));
                    break;
            }

            salesOrderHeader.SubTotal = salesOrderHeader.SalesOrderDetails.Sum(detail => detail.LineTotal);
            var freightCostAction = await calculateFreightCost(weightsFromDetails(salesOrderHeader.SalesOrderDetails), salesOrderHeader.ShipMethod);
            salesOrderHeader.Freight = freightCostAction.Value;
            if ((freightCostAction.Result as ObjectResult)?.StatusCode == (int)HttpStatusCode.BadRequest) return BadRequest(); // if freight cost calculation fails, return 400
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

            var isMailSent = this.SendConfirmationEmail(salesOrderHeader, tokenEmail, token.RawData);

            return CreatedAtAction("GetSalesOrderHeader", new { id = salesOrderHeader.SalesOrderId }, salesOrderHeader);
        }


        [HttpPost]
        [Route("weightsfromdetails")]
        public ICollection<KeyValuePair<decimal?, int>> weightsFromDetails(ICollection<SalesOrderDetail> details)
        {
            ICollection<KeyValuePair<decimal?, int>> weights = [];
            foreach(var detail in details)
            {
                var prod = _awContext.Products.Find(detail.ProductId);
                weights.Add(new KeyValuePair<decimal?, int>(prod == null ? null : prod.Weight, detail.OrderQty));
            }
            return weights;
        }

        [HttpPost]
        [Route("freightcost/{shipMethod}")]
        public async Task<ActionResult<decimal>> calculateFreightCost([FromBody]IEnumerable<KeyValuePair<decimal?, int>> weights, string shipMethod)
        {
            decimal res = 0;
            decimal costPerKg = 0.0m;
            try
            {
                using (StreamReader r = new StreamReader(Path.GetFullPath("Data\\ShippingCostsPerKg.json")))
                {
                    string json = r.ReadToEnd();
                    var costsPerKg = JsonConvert.DeserializeObject<List<KeyValuePair<string, decimal>>>(json);
                    costPerKg = costsPerKg.First(pair => pair.Key.ToLower() == shipMethod.ToLower()).Value;
                }
            }
            catch (Exception e)
            {
                await DBErrorLogger.WriteExceptionLog(_awContext, e);
                return BadRequest();
            }

            foreach (var weight in weights)
            {
                if (weight.Key != null)
                {
                    res += (decimal)weight.Key * weight.Value * (costPerKg / 1000);
                } // if the product has no weight, the weight of the parcel is fixed at 1kg
                else
                {
                    res += costPerKg * weight.Value;
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
        #endregion

    }
}
