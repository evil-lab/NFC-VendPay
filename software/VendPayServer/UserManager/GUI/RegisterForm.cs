using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using com.IntemsLab.Common.Model;
using UserManager.Devices;

namespace UserManager.GUI
{
    public partial class RegisterForm : Form
    {
        public enum FormState
        {
            Registration,
            AmountUpdate,
            Undefined
        }

        private User _user;
        private int _amountIncrement;
        private CardReader _cardReader;

        //thread safety delegate


        public RegisterForm()
        {
            InitializeComponent();
            _user = new User();
        }

        public RegisterForm(ChipCard card) : this()
        {
            var str = card.CardId;
            txtCardTag.Text = str;
        }

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }

        public int AmountIncrement
        {
            get { return _amountIncrement; }
        }

        public void SetFormState(FormState state)
        {
            switch (state)
            {
                case FormState.Registration:
                    txtName.Enabled = true;
                    txtPhone.Enabled = true;
                    txtOrganization.Enabled = true;

                    btnOk.Enabled = false;
                    btnAddAmount.Enabled = false;
                    break;

                case FormState.AmountUpdate:
                    txtName.Enabled = false;
                    txtPhone.Enabled = false;
                    txtOrganization.Enabled = false;

                    btnOk.Enabled = false;
                    btnAddAmount.Enabled = true;
                    break;
            }
        }

        public void SetAccountInfo(User user)
        {
            txtName.Text = user.UserName;
            txtPhone.Text = user.Phone;
            txtOrganization.Text = user.Organization;

            txtAmount.Text = user.Amount.ToString(CultureInfo.InvariantCulture);
        }

        public static void SetControlPropertyThreadSafe(
            Control control,
            string propertyName,
            object propertyValue)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlPropertyThreadSafeDelegate
                    (SetControlPropertyThreadSafe),
                    new[] {control, propertyName, propertyValue});
            }
            else
            {
                control.GetType().InvokeMember(
                    propertyName,
                    BindingFlags.SetProperty,
                    null,
                    control,
                    new[] {propertyValue});
            }
        }

        private void UpdateDataGroup(bool isEnable)
        {
            _user = new User();
            //SetControlPropertyThreadSafe(groupUserData, "Enabled", isEnable);
        }

        private void DataTextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Name == "txtName")
                    _user.UserName = textBox.Text.Trim();
                if (textBox.Name == "txtPhone")
                    _user.Phone = textBox.Text.Trim();
                if (textBox.Name == "txtOrganization")
                    _user.Organization = textBox.Text.Trim();
            }
            bool isComplete = CheckUserDataComplete();
            btnOk.Enabled = isComplete;
        }

        private bool CheckUserDataComplete()
        {
            bool isDataComplete;
            isDataComplete = !String.IsNullOrEmpty(_user.UserName) &
                             !String.IsNullOrEmpty(_user.Phone) &
                             !String.IsNullOrEmpty(_user.Organization);
            return isDataComplete;
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            _user = null;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void BtnAddAmountClick(object sender, EventArgs e)
        {
            int amount = int.Parse(txtAmount.Text);
            int append = int.Parse(txtAddAmount.Text);

            string amountMsg = String.Format("Пополнить счет на сумму: {0} руб?", append);
            DialogResult result = MessageBox.Show(amountMsg, "Пополнение счета", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                _amountIncrement = append;

                amount += append;
                txtAmount.Text = amount.ToString(CultureInfo.InvariantCulture);
                // close UserForm
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private delegate void SetControlPropertyThreadSafeDelegate(
            Control control,
            string propertyName,
            object propertyValue);
    }
}