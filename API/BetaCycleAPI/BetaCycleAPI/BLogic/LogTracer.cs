using BetaCycleAPI.Models;
using Microsoft.Data.SqlClient;




namespace BetaCycleAPI.BLogic
{
    public static class LogTracer
    {

        public static void AddLog(string userName, string typeTrace, string ipAddress, DateTime dateTime)
        {
            using (SqlConnection connection = new SqlConnection(Connectionstrings.AdventureWorks))
            {
                try
                {

                    connection.Open();
                    string sql = "INSERT INTO [dbo].[LogTrace] ([UserID],[TypeTrace],[IPAddress],[LogDateTime])VALUES" +
                        " (@UserID, @TyperTrace, @IPAddress, @DateTime)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", GetUserID(userName));
                        command.Parameters.AddWithValue("@TyperTrace", typeTrace);
                        command.Parameters.AddWithValue("@IPAddress", ipAddress);
                        command.Parameters.AddWithValue("@DateTime", dateTime);

                        // Eseguiamo il comando di inserimento
                        int rowsAffected = command.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore durante l'inserimento dei dati: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static long GetUserID(string username)
        {
            long id = 0;
            using (SqlConnection connection = new SqlConnection(Connectionstrings.Credentials))
            {
                try
                {

                    connection.Open();
                    string sql = "SELECT [CustomerID] FROM [dbo].[Credentials] WHERE Email = @UserMail";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserMail", username);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                id = reader.GetInt64(0);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            return id;
        }
    }
}
