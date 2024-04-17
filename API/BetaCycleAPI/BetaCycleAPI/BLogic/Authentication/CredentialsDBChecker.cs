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
    public class CredentialsDBChecker
    {
        private SqlCommand sqlCmd = new();
        private SqlConnection sqlConn = new();

        public DBCheckResponse ValidateLogin(string user, string pwd)
        {
            DBCheckResponse response = DBCheckResponse.NotFound;
            try
            {
                sqlConn.ConnectionString = Connectionstrings.Credentials;
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
