using System;
using System.Collections.Generic;
using System.IO;
using com.IntemsLab.Common.Model;
using com.IntemsLab.Common.Model.Interfaces;

namespace com.IntemsLab.Common
{
    public class FileStorageHelper : IPayments, ICardProcessor, IUserProcessor
    {
        private readonly IList<ChipCard> _cardPool;
        private readonly Dictionary<ChipCard, User> _storage;
        private int _lastId;

        private readonly object _locker;

        public const string StoragePath =  "_storage.txt";
        public const string CardPoolPath = "_cards.txt";

        public FileStorageHelper()
        {
            _locker = new object();

            _cardPool = new List<ChipCard>();
            _storage = new Dictionary<ChipCard, User>();
        }

        //card operations
        public void AddCard(ChipCard card)
        {
            if (!_cardPool.Contains(card))
            {
                _cardPool.Add(card);
                try
                {
                    using (var fs = File.Open(CardPoolPath, FileMode.Append))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(card.CardId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Card save error: {0}", ex.Message);
                    Console.WriteLine("StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else
            {
                string msg = String.Format("Card with Id:{0} already exist.", card);
                throw new CardStorageException(msg);
            }
        }

        public User AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(ChipCard card)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public bool AssignAccount(ChipCard card, User user)
        {
            bool isAssigned = false;
            lock (_locker)
            {
                if (_cardPool.Contains(card))
                {
                    user.Id = _lastId;
                    _storage[card] = user;
                    isAssigned = SaveToFile();
                    CreatePaymentFile(card);
                    _lastId++;
                }
            }
            return isAssigned;
        }

        //amount operations
        public void SaveRefill(int userId, int value)
        {
            throw new NotImplementedException();
        }

        public void SaveSale(int userId, int productId, int price)
        {
            throw new NotImplementedException();
        }

        public uint GetAmount(ChipCard card)
        {
            lock (_locker)
            {
                if (_storage.ContainsKey(card))
                {
                    return _storage[card].Amount;
                }
                var msg = String.Format("Card with Id:{0} not found", card.ToString());
                throw new CardStorageException(msg);
            }
        }

        //account operations
        public User AddUser(string name, string surname, ChipCard card)
        {
            throw new NotImplementedException();
        }

        public User GetUser(ChipCard card)
        {
            User user = null;
            if (_storage.ContainsKey(card))
                user = _storage[card];
            return user;
        }

        public KeyValuePair<ChipCard, User> GetAccountById(int id)
        {
            var result = new KeyValuePair<ChipCard, User>(null,null);
            foreach (var pair in _storage)
            {
                if (pair.Value.Id == id)
                    result = pair;
            }
            return result;
        }

        // Methods for store data to file
        private bool SaveToFile()
        {
            bool isSaved = false;
            try
            {
                using (var fs = File.Open(StoragePath, FileMode.OpenOrCreate))
                {
                    var writer = new StreamWriter(fs);
                    foreach (var val in _storage)
                    {
                        string str = val.Key.CardId + ";" + val.Value.Id + ";" + 
                                     val.Value.UserName + ";" + val.Value.Phone + ";" + val.Value.Organization + ";" + 
                                     val.Value.Amount + ";" + val.Value.SellCount;
                        writer.WriteLine(str);
                    }
                    writer.Flush();
                    writer.Close();
                    isSaved = true;
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine("Error file operation. Message: " + ioex.Message);
                Console.WriteLine("Stack trace:\r\n" + ioex.StackTrace);
            }
            return isSaved;
        }

        public void ReadFromFile()
        {
            try
            {
                using (var fs = File.Open(StoragePath, FileMode.Open))
                {
                    var reader = new StreamReader(fs);
                    while(!reader.EndOfStream)
                    {
                        string str = reader.ReadLine();
                        if (!String.IsNullOrEmpty(str))
                        {
                            string[] chunks = str.Split(';');

                            var card = new ChipCard(chunks[0]);
                            var account = new User
                            {
                                Id = Int32.Parse(chunks[1]),
                                UserName = chunks[2],
                                Phone = chunks[3],
                                Organization = chunks[4],

                                Amount = uint.Parse(chunks[5]),
                                SellCount = uint.Parse(chunks[6])
                            };
                            _lastId = account.Id+1;
                            _storage.Add(card, account);
                        }
                    }
                    reader.Close();
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine("Error file operation. Message: " + ioex.Message);
                Console.WriteLine("Stack trace:\r\n" + ioex.StackTrace);
            }

        }

        public void ReadCardPool()
        {
            using (var fs = File.Open(CardPoolPath, FileMode.Open))
            {
                var reader = new StreamReader(fs);
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if (!String.IsNullOrEmpty(str))
                    {
                        _cardPool.Add(new ChipCard(str.Trim().ToLower()));
                    }
                }
                reader.Close();
            }
        }

        public void SaveCardPool()
        {
            using (var fs = File.Open(CardPoolPath, FileMode.Append))
            {
                var writer = new StreamWriter(fs);
                foreach (var chipCard in _cardPool)
                {
                    writer.WriteLine(chipCard.CardId);
                }
            }
        }

        // Store payment operations
        private void CreatePaymentFile(ChipCard card)
        {
            try
            {
                string fileName = Path.Combine("payments", card.CardId + ".txt");
                using (var fs = File.Open(fileName, FileMode.CreateNew))
                {
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("File operation exception: {0}", ex.Message);
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void SavePayment(ChipCard card, int amount, int pos = 0)
        {
            //pos - selected vend position ({0}-undefined)
            string fileName = Path.Combine("payments", card.CardId + ".txt");
            using (var fs = File.Open(fileName, FileMode.Append))
            {
                using (var writer = new StreamWriter(fs))
                {
                    var sDate = DateTime.Now.ToLongDateString();
                    var sTime = DateTime.Now.ToLongTimeString();
                    var str = String.Format("{{{0}:{1}}};{2};{3}", sDate, sTime, amount, pos);
                    writer.WriteLine(str);
                    writer.Flush();
                }
            }
        }
    }
}
