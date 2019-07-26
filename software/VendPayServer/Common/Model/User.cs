using System;

namespace com.IntemsLab.Common.Model
{
    public class User
    {
        private ChipCard _assignedCard;

        private int _id;
        private string _userName;
        private string _phone;
        private string _organization;

        private uint _sellCount;
        private uint _amount;

        public ChipCard AssignedCard
        {
            get { return _assignedCard; }
            set { _assignedCard = value; }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        public uint SellCount
        {
            get { return _sellCount; }
            set { _sellCount = value; }
        }

        public uint Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public override string ToString()
        {
            return String.Format("Id: {0}  |  cardId: {1}  |  name: {2}", Id, AssignedCard.CardId, UserName);
        }
    }
}
