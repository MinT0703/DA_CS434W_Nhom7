using System.Configuration;
using System.Data.SqlClient;

namespace DA_CS434W
{
    public class Connection
    {
        private static string connStr =
            ConfigurationManager.ConnectionStrings["DB_Fashion"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connStr);
        }
    }
}
