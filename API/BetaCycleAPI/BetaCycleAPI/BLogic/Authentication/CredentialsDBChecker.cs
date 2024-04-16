using BetaCycleAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using EncryptData;
using BetaCycleAPI.Models.Enums;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace BetaCycleAPI.BLogic.Authentication
{
    public class CredentialsDBChecker
    {
        SqlCommand sqlCmd = new();
        SqlConnection myConn = new();
        public async Task<DBCheckResponse> ValidateLoginAsync(string user, string pwd)
        {
            DBCheckResponse response = DBCheckResponse.NotFound;
            try
            {
                dynamic appSettings = JsonConvert.DeserializeObject<string>(File.ReadAllText(@"..\..\appsettings.json"));
                myConn.ConnectionString = appSettings.ConnectionString;
                //if (credLoginNew.Any()) //new db
                //{
                //    if ((EncryptData.CypherData.DecryptSalt(pwd, credLoginNew.First().SaltHash)).Equals(credLoginNew.First().PasswordHash))
                //    {
                //        response = DBCheckResponse.FoundMigrated;
                //    }
                //}
                //else if (credLoginOld.Any()) //old db
                //{
                //    response = DBCheckResponse.FoundNotMigrated;
                //}
            }catch(Exception ex)
            {
                throw;
            }
            return response;
        }
    }
}
