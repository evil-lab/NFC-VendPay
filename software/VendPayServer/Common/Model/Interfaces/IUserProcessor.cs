using System.Collections.Generic;

namespace com.IntemsLab.Common.Model.Interfaces
{
    public interface IUserProcessor
    {
        User AddUser(User user);
        void DeleteUser(ChipCard card);

        User GetUser(ChipCard card);
        User GetUserById(int userId);

        bool AssignAccount(ChipCard card, User user);
        KeyValuePair<ChipCard, User> GetAccountById(int id);
    }
}