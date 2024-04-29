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
    /// <summary>
    /// Static class with utilities for checking credentials' validity in the DB
    /// </summary>
    public static class CredentialsDBChecker
    {
        public static async Task<DBCheckResponse> ValidateLogin(string user, string pwd)
        {
            SqlCommand sqlCmd = new();
            SqlConnection sqlConn = new(Connectionstrings.Credentials);

            DBCheckResponse response = DBCheckResponse.NotFound;
            try
            {
                await sqlConn.OpenAsync();

                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.Parameters.AddWithValue("@Email", user);
                sqlCmd.CommandText = "SELECT TOP 1 Email, PasswordHash, SaltHash, AdminPermission FROM [dbo].[Credentials] WHERE Email = @Email";
                sqlCmd.Connection = sqlConn;
                using(SqlDataReader reader = await sqlCmd.ExecuteReaderAsync())
                {
                    if(reader.Read()) {
                        if (EncryptData.CypherData.DecryptSalt(pwd, reader["SaltHash"].ToString()).Equals(reader["PasswordHash"].ToString()))
                        {
                            response = reader["AdminPermission"].ToString() == "True" ? DBCheckResponse.FoundAdmin : DBCheckResponse.FoundMigrated;
                        }
                    }
                    await sqlConn.CloseAsync();
                }

                sqlConn.ConnectionString = Connectionstrings.AdventureWorks;
                await sqlConn.OpenAsync();

                sqlCmd.CommandText = "SELECT TOP 1 EmailAddress FROM [SalesLT].[Customer] WHERE EmailAddress = @Email";
                sqlCmd.Connection = sqlConn;

                using (SqlDataReader reader = await sqlCmd.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        response = DBCheckResponse.FoundNotMigrated;
                        if (reader["EmailAddress"].ToString() == string.Empty) response = DBCheckResponse.NotFound;
                    }
                    Console.WriteLine();
                }

                await sqlConn.CloseAsync();


            }
            catch(Exception ex)
            {
                throw;
            }
            return response;
        }
    }
}
