using System.IO;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using com.IntemsLab.Common.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace com.intemslab.DbApiTests
{
    [TestClass]
    public class PaymentMethodsTest
    {
        private const string DatabaseName = "unit_test.db";

        private readonly DatabaseGenerator _generator = new DatabaseGenerator(DatabaseName);
        private IUserProcessor _processor;
        private IPayments _payments;


        [ClassCleanup]
        public static void Finish()
        {
            if (File.Exists(DatabaseName))
                File.Delete(DatabaseName);
        }

        [TestInitialize]
        public void Setup()
        {
            _generator.Create();

            _processor = new DatabaseHelper(DatabaseName);
            ((DatabaseHelper)_processor).Start();
            _payments = (IPayments) _processor;
        }

        [TestCleanup]
        public void TearDown()
        {
            _generator.Clear();
        }

        [TestMethod]
        public void GetUserAmountTest()
        {
            var card = new ChipCard("aabbccdd");
            var user = new User {AssignedCard = card, UserName = "User1", Amount = 100};

            _processor.AddUser(user);
            var result = _payments.GetAmount(card);

            Assert.AreEqual(user.Amount, result);
        }

        [TestMethod]
        public void SaveSaleTest()
        {
            var card = new ChipCard("aabbccdd");
            var user = new User {AssignedCard = card, UserName = "User1", Amount = 100};

            var userId = _processor.AddUser(user).Id;
            _payments.SaveSale(userId, 1, 20);

            uint expected = 80;
            var result = _processor.GetUserById(userId).Amount;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SaveRefillTest()
        {
            var card = new ChipCard("aabbccdd");
            var user = new User() {AssignedCard = card, UserName = "User1", Amount = 100};

            var userId = _processor.AddUser(user).Id;
            _payments.SaveRefill(userId, 10);

            uint expected = 110;
            var result = _processor.GetUserById(userId).Amount;
            Assert.AreEqual(expected, result);
        }
    }
}
