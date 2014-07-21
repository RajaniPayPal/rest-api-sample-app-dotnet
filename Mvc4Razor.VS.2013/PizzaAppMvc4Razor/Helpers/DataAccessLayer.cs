using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace PizzaAppMvc4Razor
{
    class DataAccessLayer
    {
        private string ConnectionString
        {
            get
            {
                ConnectionStringSettingsCollection connectionStringSettings = ConfigurationManager.ConnectionStrings;
                return connectionStringSettings["ConnectionString"].ConnectionString;
            }
        }

        public DataTable Select(SQLiteCommand commandSQLite)
        {
            DataTable datTable = null;
            try
            {
                using (SQLiteConnection connectionSQLite = new SQLiteConnection(ConnectionString))
                {
                    commandSQLite.Connection = connectionSQLite;
                    commandSQLite.Connection.Open();
                    datTable = new DataTable();
                    datTable.Load(commandSQLite.ExecuteReader());
                    return datTable;
                }
            }
            finally
            {
                datTable = null;
            }
        }

        public int Execute(SQLiteCommand commandSQLite)
        {
            int rowsAffected = 0;
            using (SQLiteConnection connectionSQLite = new SQLiteConnection(ConnectionString))
            {
                commandSQLite.Connection = connectionSQLite;
                commandSQLite.Connection.Open();
                rowsAffected = commandSQLite.ExecuteNonQuery();
            }
            return rowsAffected;
        }
    }
}
