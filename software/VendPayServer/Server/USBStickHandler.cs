using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Csv;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using NLog;
using System.Text.RegularExpressions;

namespace com.IntemsLab.Server
{
    public class USBStickHandler
    {
        private DatabaseHelper _dbHelper;
        private bool _isProcessing;
        private string _baseDir = "/mnt/vend_pay";
        private string[] _configFiles = { "cards.csv", "balance.csv" };
        private readonly Logger _log;

        public event EventHandler Configuring;
        public event EventHandler Configured;

        public USBStickHandler(DatabaseHelper dbHelper)
        {
            _log = LogManager.GetLogger("fileLogger");
            _dbHelper = dbHelper;
            _isProcessing = true;
        }

        public void Start()
        {
            Processing();
        }

        private void Processing()
        {
            while(_isProcessing)
            {
                var cardsFile = Path.Combine(_baseDir, _configFiles[0]);
                var balanceFile = Path.Combine(_baseDir, _configFiles[1]);
                if (File.Exists(cardsFile))
                {
                    Configuring?.Invoke(this, EventArgs.Empty);
                    GetCards(cardsFile);
                    Configured?.Invoke(this, EventArgs.Empty);
                }
                if(File.Exists(balanceFile))
                {
                    Configuring?.Invoke(this, EventArgs.Empty);
                    GetBalance(balanceFile);
                    Configured?.Invoke(this, EventArgs.Empty);
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public bool IsValidTime(string t)
        {
            Regex checker = new Regex(@"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$");
            return checker.IsMatch(t);
        }


        private void GetBalance(string balanceFile)
        {
            var csv = File.ReadAllText(balanceFile);
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                var s_balance = line["balance"];
                var time = line["time"];
                // check balance
                int balance;
                if (Int32.TryParse(s_balance, out balance) && IsValidTime(time)) 
                {
                    _log.Debug("read: balance: {0}, time: {1}", balance, time);
                    Tuple<int, string> lastConf = _dbHelper.GetLastConfig();

                    if (lastConf == null)
                    {
                        _log.Debug("inserted: balance: {0}, time: {1}", balance, time);
                        _dbHelper.SaveConfig(balance, time);
                    }
                    else
                    {
                        if (!(lastConf.Item1 == balance && lastConf.Item2 == time))
                        {
                            _log.Debug("inserted: balance: {0}, time: {1}", lastConf.Item1, lastConf.Item2);
                            _dbHelper.SaveConfig(balance, time);
                        }
                        else
                        {
                            _log.Debug("already in: balance: {0}, time: {1}", lastConf.Item1, lastConf.Item2);
                        }
                    }
                }
                else
                {
                    _log.Debug("incorrect format: balance: {0}, time: {1}", s_balance, time);
                }
            }
        }

        private void GetCards(string cardsFile)
        {
            var csv = File.ReadAllText(cardsFile);
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                var card_id = line["card_id"];

                ChipCard card = new ChipCard(card_id.ToUpper().Trim());
                var usr = _dbHelper.GetUser(card);
                if (usr != null) 
                {
                    _log.Debug("User for: {0} already in db", card_id);
                }
                else
                {
                    _log.Debug("No user for: {0}", card_id);
                    var newUser = _dbHelper.AddUser(new User() { AssignedCard = card, UserName = card.CardId });
                    if (newUser.Id > 0)
                    {
                        _log.Debug("User for: {0} added into DB", card_id);
                    }
                    else 
                    {
                        _log.Debug("User for: {0} NOT added into DB", card_id);
                    }
                }
            }
        }
    }
}
