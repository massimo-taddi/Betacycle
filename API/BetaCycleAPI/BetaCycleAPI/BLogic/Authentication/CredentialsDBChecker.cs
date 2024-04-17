using BetaCycleAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using EncryptData;
using BetaCycleAPI.Models.Enums;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using BetaCycleAPI.Models;
using Microsoft.CodeAnalysis.Options;

namespace BetaCycleAPI.BLogic.Authentication
{
    public static class CredentialsDBChecker
    {
        public static DBCheckResponse ValidateLogin(string user, string pwd)
        {
            SqlCommand sqlCmd = new();
            SqlConnection sqlConn = new(Connectionstrings.Credentials);

            DBCheckResponse response = DBCheckResponse.NotFound;
            try
            {
                sqlConn.Open();

                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@Email", user);
                sqlCmd.CommandText = "SELECT TOP 1 Email, PasswordHash, SaltHash, AdminPermission FROM [dbo].[Credentials] WHERE Email = @Email";
                sqlCmd.Connection = sqlConn;

                using(SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    if(reader.Read()) {
                        if (EncryptData.CypherData.DecryptSalt(pwd, reader["SaltHash"].ToString()).Equals(reader["PasswordHash"].ToString()))
                        {
                            response = reader["AdminPermission"].ToString() == "True" ? DBCheckResponse.FoundAdmin : DBCheckResponse.FoundMigrated;
                        }
                    }
                    sqlConn.Close();
                }

                sqlConn.ConnectionString = Connectionstrings.AdventureWorks;
                sqlConn.Open();

                sqlCmd.CommandText = "SELECT TOP 1 EmailAddress FROM [SalesLT].[Customer] WHERE EmailAddress = @Email";
                sqlCmd.Connection = sqlConn;

                using (SqlDataReader reader = sqlCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        response = DBCheckResponse.FoundNotMigrated;
                    }
                }


            }
            catch(Exception ex)
            {
                throw;
            }
            return response;
        }
    }
}
