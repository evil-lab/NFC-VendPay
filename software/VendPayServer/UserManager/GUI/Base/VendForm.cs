using System.Windows.Forms;
using com.IntemsLab.Common;
using UserManager.Devices;

namespace UserManager.GUI.Base
{
    public class VendForm : Form
    {
        private FileStorageHelper _storage;
        private CardReader _reader;


        public VendForm()
        {
        }

        public VendForm(FileStorageHelper storage) : this()
        {
            _storage = storage;
        }

        public FileStorageHelper Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }

        public CardReader Reader
        {
            get { return _reader; }
            set { _reader = value; }
        }

        // protected methods
        protected void ShowOnScreen(int screenNumber)
        {
            var screens = Screen.AllScreens;

            if (screenNumber >= 0 && screenNumber < screens.Length)
            {
                bool maximised = false;
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                    maximised = true;
                }
                Location = screens[screenNumber].WorkingArea.Location;
                if (maximised)
                {
                    WindowState = FormWindowState.Maximized;
                }
            }
        }
    }
}
