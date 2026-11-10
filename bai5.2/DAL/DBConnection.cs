
using System;
using Microsoft.Data.SqlClient;

namespace KaraokeManagement.DAL
{
    public class DBConnection
    {
        private static readonly string connectionString =
            @"Data Source=.;Initial Catalog=KaraokeDB;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
