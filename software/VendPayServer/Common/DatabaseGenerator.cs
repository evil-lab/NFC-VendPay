using System.Data;
using System.Data.SQLite;
using System.IO;

namespace com.IntemsLab.Common
{
    public class DatabaseGenerator
    {
        private const string Prefix = "Data Source = ";

        private readonly string _databaseName;

        public DatabaseGenerator(string databaseName)
        {
            _databaseName = databaseName;
        }

        public void Create()
        {
            if (!File.Exists(_databaseName))
            {
                SQLiteConnection.CreateFile(_databaseName);
                //Create();
            }
            // create tables
            using (var conn = new SQLiteConnection(Prefix + _databaseName))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"CREATE TABLE [User] (
                                            [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            [cardId] char(25) NOT NULL,
                                            [name] char(100),
                                            [organization] char(250),
                                            [phone] char(20),
                                            [amount] integer );";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    //DateTime format
                    //"YYYY-MM-DD HH:MM:SS.SSS"
                    cmd.CommandText = @"CREATE TABLE [Purchase] (
                                            [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                                            [userId] integer,
                                            [date] char(25),
                                            [value] integer,
                                            [cellId] integer);";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Clear()
        {
            using (var conn = new SQLiteConnection(Prefix + _databaseName))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "DROP TABLE [Purchase]";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE [User]";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
