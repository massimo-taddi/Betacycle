using BetaCycleAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using EncryptData;
using BetaCycleAPI.Models.Enums;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using BetaCycleAPI.Models;

namespace BetaCycleAPI.BLogic.Authentication
{
    public class CredentialsDBChecker
    {
        SqlCommand sqlCmd = new();
        SqlConnection myConn = new();

        public DBCheckResponse ValidateLogin(string user, string pwd)
        {
            DBCheckResponse response = DBCheckResponse.NotFound;
            try
            {
                myConn.ConnectionString = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("appsettings.json")).ConnectionStrings.AdventureWorks;
                
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
            }
            catch(Exception ex)
            {
                throw;
            }
            return response;
        }
    }
}
