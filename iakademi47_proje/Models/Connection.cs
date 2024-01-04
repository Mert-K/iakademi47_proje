using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Data.SqlClient;

namespace iakademi47_proje.Models
{
    public class Connection
    {
        public static SqlConnection ServerConnect
        {
            get
            {
                SqlConnection sqlConnection = new SqlConnection("Server=localhost\\SQLEXPRESS;Database=iakademi47Core_Proje;Trusted_Connection=True;TrustServerCertificate=True;");
                return sqlConnection;
            }
        }
    }
}
