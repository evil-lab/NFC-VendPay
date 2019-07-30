using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Globalization;
using Mono.Data.Sqlite;
using com.IntemsLab.Common.Model;
using com.IntemsLab.Common.Model.Interfaces;

namespace com.IntemsLab.Common
{
    public class DatabaseHelper : IUserProcessor, IPayments, IDisposable
    {
        private readonly string _databaseName = "vend_users.db";
        private readonly object _locker = new object();

        private const string ConnStringTemplate = "Data Source={0};Version=3;PRAGMA quick_check= NORMAL;";

        //private SQLiteConnection _connection;
        private SqliteConnection _connection;

        public DatabaseHelper(string dbName)
        {
            _databaseName = dbName;
        }

        public void Start()
        {
            if (!File.Exists(_databaseName))
            {
                lock (_locker)
                {
                    CreateDatabase();
                }
            }
            var connectionString = String.Format(ConnStringTemplate, _databaseName);
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        public User AddUser(User user)
        {
            User result;
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    if (IsCardExists(user.AssignedCard)) return null;

                    string text = "INSERT INTO [User] (cardId, name, organization, phone, amount) " +
                                  String.Format("VALUES('{0}', '{1}', '{2}', '{3}', {4})",
                                  user.AssignedCard.CardId, user.UserName, user.Organization, user.Phone, user.Amount);

                    cmd.CommandText = text;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    result = GetUser(user.AssignedCard);
                }
            }
            return result;
        }

        public void DeleteUser(ChipCard card)
        {
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    var sqlTemplate = "DELETE FROM [User] WHERE cardId LIKE '{0}'";
                    string sCmd = String.Format(sqlTemplate, card.CardId);

                    cmd.CommandText = sCmd;
                    cmd.CommandType = CommandType.Text;
                    var count = cmd.ExecuteNonQuery();
                }
            }
        }

        public User GetUser(ChipCard card)
        {
            User result = null;
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    var sqlTemplate = "SELECT * FROM User WHERE cardId LIKE '{0}'";
                    var sCmd = String.Format(sqlTemplate, card.CardId);

                    cmd.CommandText = sCmd;
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var userId = reader.GetInt32(0);
                        var name = reader.GetString(2);
                        var organization = reader.GetString(3);
                        var phone = reader.GetString(4);
                        var amount = reader.GetInt32(5);

                        result = new User
                        {
                            Id = userId,
                            AssignedCard = card,
                            UserName = name,
                            Organization = organization,
                            Phone = phone,
                            Amount = (uint)amount
                        };
                    }
                }
            }
            return result;
        }

        public User GetUserById(int anUserId)
        {
            User result = null;
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    var sqlTemplate = "SELECT * FROM User WHERE Id = {0}";
                    var sCmd = String.Format(sqlTemplate, anUserId);

                    cmd.CommandText = sCmd;
                    cmd.CommandType = CommandType.Text;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var userId = reader.GetInt32(0);
                        var cardId = reader.GetString(1);
                        var name = reader.GetString(2);
                        var organization = reader.GetString(3);
                        var phone = reader.GetString(4);
                        var amount = reader.GetInt32(5);

                        result = new User
                        {
                            Id = userId,
                            AssignedCard = new ChipCard(cardId),
                            UserName = name,
                            Organization = organization,
                            Phone = phone,
                            Amount = (uint)amount
                        };
                    }
                }
            }
            return result;
        }

        public bool AssignAccount(ChipCard card, User user)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<ChipCard, User> GetAccountById(int id)
        {
            throw new NotImplementedException();
        }

        //----
        public uint GetAmount(ChipCard card)
        {
            uint result;

            var user = GetUser(card);
            if (user != null)
                result = user.Amount;
            else
                throw new Exception("User not found");

            return result;
        }

        public void SaveSale(int userId, int productId, int price)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                if (user.Amount >= price)
                {
                    var newAmount = (int) (user.Amount - price);
                    UpdateAmount(userId, newAmount);
                    SavePurchase(userId, price, productId);
                }
            }
        }

        public void SaveRefill(int userId, int value)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                var newAmount = (int) (user.Amount + value);
                UpdateAmount(userId, newAmount);
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        //PRIVATE METHODS
        private bool IsCardExists(ChipCard card)
        {
            var result = false;
            if (_connection != null)
            {
                lock (_locker)
                {
                    using (var cmd = new SqliteCommand(_connection))
                    {
                        const string sqlQryTempl = @"SELECT id FROM User WHERE cardId LIKE '{0}'";
                        cmd.CommandText = String.Format(sqlQryTempl, card.CardId);
                        cmd.CommandType = CommandType.Text;
                        var qryResult = cmd.ExecuteScalar();
                        result = qryResult != null;
                    }
                }
            }
            return result;
        }

        public void SaveConfig(int balance, string updateTime)
        {
            lock(_locker)
            {
                using (var cmd = new SqliteCommand(_connection)) {
                    const string sqlTemplate = "INSERT INTO [Config] ([balance], [updateTime])" +
                                              "VALUES ({0}, '{1}')";
                    var sCmd = String.Format(sqlTemplate, balance, updateTime);

                    cmd.CommandText = sCmd;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Tuple<int, string> GetLastConfig()
        {
            Tuple<int, string> result = null;
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    const string sqlQryTempl = @"SELECT * FROM Config ORDER BY [id] DESC LIMIT 1";
                    cmd.CommandText = String.Format(sqlQryTempl);
                    cmd.CommandType = CommandType.Text;

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var balance = reader.GetInt32(1);
                        var updateTime = reader.GetString(2);

                        result = new Tuple<int, string>(balance, updateTime);
                    }
                }
            }
            return result;
        }

        private void UpdateAmount(int userId, int amount)
        {
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    const string sqlTemplate = "UPDATE [User] SET [amount] = {1} WHERE id = {0}";
                    var sCmd = String.Format(sqlTemplate, userId, amount);

                    cmd.CommandText = sCmd;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SavePurchase(int userId, int value, int cellId)
        {
            lock (_locker)
            {
                using (var cmd = new SqliteCommand(_connection))
                {
                    const string sqlTemplate = "INSERT INTO [Purchase] ([userId], [date], [value], [cellId])" +
                                               "VALUES ({0}, '{1}', {2}, {3})";
                    var sCmd = String.Format(sqlTemplate, userId, DateTime.Now.ToString("u"), value, cellId);

                    cmd.CommandText = sCmd;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CreateDatabase()
        {
            SqliteConnection.CreateFile(_databaseName);
            using (var conn = new SqliteConnection("Data Source = " + _databaseName + ";Version=3;"))
            {
                conn.Open();
                using (var cmd = new SqliteCommand(conn))
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

                    cmd.CommandText = @"CREATE TABLE [Config] (
                                        [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                                        [balance] integer,
                                        [updateTime] char(25));";
                    cmd.ExecuteNonQuery();
                }
            } 
        }
    }
}
