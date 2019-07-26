namespace com.IntemsLab.Common.Model.Interfaces
{
    public interface IPayments
    {
        uint GetAmount(ChipCard card);
        void SaveSale(int userId, int productId, int price);
        void SaveRefill(int userId, int value);
    }
}