using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Csv;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using NLog;
using System.Text.RegularExpressions;
using System.Globalization;

namespace com.IntemsLab.Server
{
    public class USBStickHandler
    {
        private DatabaseHelper _dbHelper;
        private bool _isProcessing;
        private string _baseDir;
        private string _reportArchivesDir;
        private string[] _configFiles = { "cards.csv", "balance.csv"};
        private readonly Logger _log;

        public event EventHandler Configuring;
        public event EventHandler Configured;

        public USBStickHandler(DatabaseHelper dbHelper, string baseDir, string reportArchiveDir)
        {
            _baseDir = baseDir;
            _reportArchivesDir = reportArchiveDir;
            _dbHelper = dbHelper;

            _log = LogManager.GetLogger("fileLogger"); 
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

                if(Directory.Exists(_baseDir))
                {
                    Configuring?.Invoke(this, EventArgs.Empty);
                    SaveCSVReport();
                    _dbHelper.SaveReportLog(); 
                    Configured?.Invoke(this, EventArgs.Empty);
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public bool IsValidTime(string t)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(t, @"^([01]\d?|2[0-4]):[0-5]\d(:[0-5]\d)?$");
        }

        public bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
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
                    _log.Debug("INCORRECT balance: {0}, time: {1}", s_balance, time);
                }
            }
        }

        private void SaveCSVReport()
        {
            // get last config time 
            string lastReportLogTime = _dbHelper.GetLastReportLog();
            // check if last config time is null
            if (lastReportLogTime == null)
                lastReportLogTime = "2000-01-01 00:00:00Z";
            
            _log.Debug("ReportLog: last report log time is: {0}", lastReportLogTime);
            var data = _dbHelper.GetReport(lastReportLogTime);
            if (data.Count > 0)
            {
                try
                {
                    string reportFile = Path.Combine(_baseDir, String.Format("report {0}.csv", DateTime.Now.ToString("u")));
                    string reportArchiveDir = Path.Combine(_reportArchivesDir, String.Format("report {0}.csv", DateTime.Now.ToString("u")));
                    List<string[]> lines = new List<string[]>();
                    foreach (var tpl in data)
                    {
                        lines.Add(new string[] { tpl.Item1, tpl.Item2.ToString(), tpl.Item3.ToString(), tpl.Item4 });
                    }
                    using (TextWriter writer = new StreamWriter(File.OpenWrite(reportFile)))
                    {
                        CsvWriter.Write(writer, new string[] { "cardId", "cellId", "price", "date" }, lines);
                    }
                    // duplicate localy
                    using (TextWriter writer = new StreamWriter(File.OpenWrite(reportArchiveDir)))
                    {
                        CsvWriter.Write(writer, new string[] { "cardId", "cellId", "price", "date" }, lines);
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("SaveCSVReport() | Message:{0}", ex.Message);
                }
            }
            
        }

        private void GetCards(string cardsFile)
        {
            var csv = File.ReadAllText(cardsFile);
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                var card_id = line["card_id"];
                if (OnlyHexInString(card_id))
                {
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
                else 
                {
                    _log.Debug("INCORRECT card id: {0}", card_id);
                }
            }
        }
    }
}
