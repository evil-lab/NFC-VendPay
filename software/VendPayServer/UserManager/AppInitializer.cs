using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using UserManager.Devices;
using UserManager.GUI;
using UserManager.Server;

namespace UserManager
{
    public class AppInitializer
    {
        private readonly CardReader _configReader;
        //main form reference
        private readonly MainForm _mainForm;
        private readonly CLServer _server;
        private readonly FileStorageHelper _storage;

        public AppInitializer()
        {
            string portName = ReadPortName();
            _configReader = new CardReader(portName);
            _storage = new FileStorageHelper();
            _server = new CLServer(_storage);

            _mainForm = new MainForm();
        }

        public Form MainForm
        {
            get { return _mainForm; }
        }

        public Tuple<bool, bool> Init()
        {
            bool isCardStorageInit;
            bool isCardReaderInit;
            try
            {
                isCardStorageInit = InitCardStorage();
                isCardReaderInit = InitCardReader();
                //initialize server
                _server.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return new Tuple<bool, bool>(isCardStorageInit, isCardReaderInit);
        }

        private string ReadPortName()
        {
            const string fileName = "app.cfg";

            string result = String.Empty;
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Can't find appication config.");
                return result;
            }
            // read port name
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                using (var reader = new StreamReader(fs))
                {
                    result = reader.ReadLine();
                }
            }
            return result;
        }

        private bool InitCardStorage()
        {
            bool isInit = false;
            try
            {
                _storage.ReadCardPool();
                _storage.ReadFromFile();
                isInit = true;
            }
            catch (IOException ioex)
            {
                Console.WriteLine("File system exception: " + ioex.Message);
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ioex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception: " + ex.Message);
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
            }
            return isInit;
        }

        private bool InitCardReader()
        {
            bool isInit = false;
            try
            {
                _configReader.Init();
                _configReader.TagReceived += OnCardReceived;
                isInit = true;
            }
            catch (IOException ioex)
            {
                Console.WriteLine("Serial port exception: " + ioex.Message);
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ioex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception: " + ex.Message);
                Console.WriteLine("Stack trace:");
                Console.WriteLine(ex.StackTrace);
            }
            return isInit;
        }

        private void OnCardReceived(object sender, CardReader.CardReaderEventArgs args)
        {
            _configReader.StopListening();

            RegisterForm.FormState state;

            var userForm = new RegisterForm(args.Card);
            var user = _storage.GetUser(args.Card);
            if (user != null)
            {
                state = RegisterForm.FormState.AmountUpdate;
                userForm.SetAccountInfo(user);
                _mainForm.SetAccountInfo(args.Card, user);
            }
            else
            {
                state = RegisterForm.FormState.Registration;
            }

            userForm.SetFormState(state);
            DialogResult result = userForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                var account = userForm.User;
                if (_storage != null)
                {
                    switch (state)
                    {
                        case RegisterForm.FormState.Registration:
                            _storage.AddCard(args.Card);
                            _storage.AssignAccount(args.Card, account);
                            break;
                        case RegisterForm.FormState.AmountUpdate:
                            _storage.SaveRefill(user.Id, userForm.AmountIncrement);
                            var updatedAccount = _storage.GetUser(args.Card);
                            _mainForm.SetAccountInfo(args.Card, updatedAccount);
                            break;
                    }
                }
            }
            //show amount to user
            Thread.Sleep(3000);
            _mainForm.ResetInfoWidgets();

            _configReader.StartListening();
        }
    }
}