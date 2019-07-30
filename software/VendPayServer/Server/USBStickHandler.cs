using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Csv;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using NLog;

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
                if (File.Exists(cardsFile))
                {
                    Configuring?.Invoke(this, EventArgs.Empty);
                    GetCards(cardsFile);
                    Configured?.Invoke(this, EventArgs.Empty);
                }
                System.Threading.Thread.Sleep(1000);
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
