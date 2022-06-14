using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text.Json;

namespace DALMSSQL
{
    public class ConnectionDb
    {

        public SqlConnection? connection;
        private readonly string connectionString;

        public ConnectionDb(string con)
        {
            this.connectionString = con;
        }

        public void OpenConnection()
        {
            connection = new SqlConnection(this.connectionString);
            connection.Open();
        }

        public void CloseConnetion()
        {
            if (connection != null)
                connection.Close();
        }
    }
}
