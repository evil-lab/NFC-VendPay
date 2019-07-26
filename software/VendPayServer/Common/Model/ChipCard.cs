namespace com.IntemsLab.Common.Model
{
    public class ChipCard
    {
        private readonly string _cardId;

        public ChipCard()
        {}

        public ChipCard(string cardId)
        {
            _cardId = cardId;
        }

        public string CardId
        {
            get { return _cardId; }
        }

        public override bool Equals(object obj)
        {
            var isEqual = false;
            var newCard = obj as ChipCard;
            if (newCard != null)
                isEqual = _cardId.Equals(newCard._cardId);
            return isEqual;
        }

        public override int GetHashCode()
        {
            return _cardId.GetHashCode();
        }

        public override string ToString()
        {
            return _cardId;
        }
    }
}
