using System;
using System.Globalization;
using System.Windows.Forms;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using UserManager.GUI.Base;

namespace UserManager.GUI
{
    public partial class MainForm : VendForm
    {
        private const int ScreenNum = 1;
        private const string CardIdBlank = "XX-XX-XX-XX-XX-XX-XX";
        private readonly Timer _timer = new Timer {Interval = 3000};

        public MainForm()
        {
            InitializeComponent();

            ShowFullscreen();
            ShowOnScreen(ScreenNum);
        }

        public MainForm(FileStorageHelper storage) : base(storage)
        {
            InitializeComponent();

            ShowFullscreen();
            ShowOnScreen(ScreenNum);
        }

        public void SetAccountInfo(ChipCard card, User user)
        {
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblCardId, "Text", card.CardId);
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblUser, "Text", user.UserName);
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblOrganization, "Text", user.Organization);
            var sAmount = user.Amount.ToString(CultureInfo.InvariantCulture);
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblAmount, "Text", sAmount);
        }

        public void ResetInfoWidgets()
        {
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblCardId, "Text", CardIdBlank);
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblUser, "Text", "");
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblOrganization, "Text", "");
            ThreadSafetyHelper.SetControlPropertyThreadSafe(lblAmount, "Text", "");
        }

        public void ClearWithPause(int time)
        {
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            ResetInfoWidgets();
        }

        private void ShowFullscreen()
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        private void BtnRegFormClick(object sender, EventArgs e)
        {
            var form = new RegisterForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var account = form.User;
            }
        }
    }
}