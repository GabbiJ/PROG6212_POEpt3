using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary
{
    /// <summary>
    /// Class that contains method that creates a connection to the database 
    /// </summary>
    public class Connections
    {
        /// <summary>
        /// Returns a connection using a connection string that connects the application to an azure database
        /// </summary>
        /// <returns>SqlConnection to Azure</returns>
        public static SqlConnection GetConnection()
        {
            string strCon = @"Server=tcp:st10034968server.database.windows.net,1433;Initial Catalog=ST10034968_PROG6212_POE;Persist Security Info=False;User ID=ST10034968;Password=@Dvtech123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            return new SqlConnection(strCon);
        }
    }
}
