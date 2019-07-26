using System.IO;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using com.IntemsLab.Common.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace com.intemslab.DbApiTests
{
    [TestClass]
    public class UserProcessorMethodsTest
    {
        private const string DatabaseName = "unit_test.db";

        private readonly DatabaseGenerator _generator = new DatabaseGenerator(DatabaseName);
        private IUserProcessor _processor;


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
        }

        [TestCleanup]
        public void TearDown()
        {
            _generator.Clear();
        }

        [TestMethod]
        public void AddUserTest()
        {
            var card = new ChipCard("aabbccdd");
            var user = new User {AssignedCard = card, Organization = "Org1"};

            var userId = _processor.AddUser(user).Id;
            var result = _processor.GetUser(card);

            Assert.AreEqual(userId, result.Id);
            Assert.AreEqual("Org1", result.Organization);
        }

        [TestMethod]
        public void AddExistedUserTest()
        {
            var card = new ChipCard("aabbccdd");
            var user1 = new User {AssignedCard = card, UserName = "User1", Organization = "Org1"};
            var user2 = new User {AssignedCard = card, UserName = "User2", Organization = "Org2"};

            var u1 = _processor.AddUser(user1);
            var result = _processor.AddUser(user2);

            Assert.AreEqual(user1.AssignedCard, u1.AssignedCard);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveUserTest()
        {
            var card = new ChipCard("aabbccdd");
            var user = new User {AssignedCard = card, Organization = "Org1"};

            var userId = _processor.AddUser(user);
            _processor.DeleteUser(card);
            var result = _processor.GetUser(card);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetUserByIdTest()
        {
            var card = new ChipCard("aabbccdd");
            var user = new User {AssignedCard = card, Organization = "Org1"};

            var userId = _processor.AddUser(user).Id;
            var result = _processor.GetUserById(userId);

            Assert.AreEqual(userId, result.Id);
            Assert.AreEqual(card.CardId, result.AssignedCard.CardId);
            Assert.AreEqual(user.Organization, result.Organization);
        }
    }
}