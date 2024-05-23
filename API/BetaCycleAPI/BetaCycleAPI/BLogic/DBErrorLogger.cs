using BetaCycleAPI.Contexts;
using BetaCycleAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BetaCycleAPI.BLogic
{
    public static class DBErrorLogger
    {
        public static async Task<IActionResult> WriteExceptionLog(AdventureWorksLt2019Context awContext, Exception e)
        {
            ErrorLog err = new ErrorLog()
            {
                ErrorTime = DateTime.Now,
                ErrorNumber = e.HResult,
                ErrorProcedure = e.TargetSite != null ? e.TargetSite.Name : null,
                ErrorMessage = e.Message,
                ErrorSource = "BetaCycleAPI"
            };
            awContext.ErrorLogs.Add(err);
            await awContext.SaveChangesAsync();
            return new OkResult();
        }
    }
}
