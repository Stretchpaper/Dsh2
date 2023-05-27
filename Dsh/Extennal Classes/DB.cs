using MySql.Data.MySqlClient;

namespace Dsh.Extennal_Classes
{
    class DB
    {
        private MySqlConnection conn;

        public DB()
        {
            conn = new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=bruh");
        }

        public void OpenConnection()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
                conn.Open();
        }

        public void CloseConnection()
        {
            if (conn.State == System.Data.ConnectionState.Open)
                conn.Close();
        }

        public MySqlConnection GetConnection()
        {
            return conn;
        }
    }
}
