using System;
using System.Net;
using System.Net.Sockets;
using com.IntemsLab.Communication;
using com.IntemsLab.Communication.Protocol;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;

namespace ServerTest
{
    class Program
    {
        private static void SendMessage(byte[] buffer)
        {
            var client = new TcpClient();
            client.Connect(IPAddress.Loopback, 6767);
            using (var stream = client.GetStream())
            {
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            client.Close();
        }

        private static void TestUserBalance()
        {
            const byte cmdCode = 0x01;
            const string uid = "04cb212c06b000";

            var buffer = new byte[uid.Length + 2]; //тип карты (1байт) конец строки (1байт)
            var requestBytes = new Bytes(buffer);
            requestBytes.WriteByte(cmdCode);
            requestBytes.WriteString(uid);

            Console.WriteLine("SEND GetUserInfo message: CODE={0} UID={1}", cmdCode, uid);

            var frame = new RequestFrame(1, 1, 0x01, requestBytes);
            SendMessage(frame.GetBytes());
        }

        private static void TestMakeSale(object obj)
        {
            byte  cmdCode = 0x02;
            Int32 userId = obj is int ? (int) obj : 0;
            Int32 productId = 1;
            Int32 productPrice = 4000; //40RUB

            const int length = sizeof (byte) + sizeof (Int32)*3;
            var buffer = new byte[length];
            var requestBytes = new Bytes(buffer);
            //requestBytes.WriteByte(cmdCode);
            requestBytes.WriteInt32(userId);
            requestBytes.WriteInt32(productId);
            requestBytes.WriteInt32(productPrice);

            Console.WriteLine("SEND MakeSale message: CODE={0} userId={1} productId={2} price={3}", cmdCode, userId,
                productId, productPrice);

            var frame = new RequestFrame(1, 2, cmdCode, requestBytes);
            SendMessage(frame.GetBytes());
        }

        static void Main(string[] args)
        {
            var service = new DatabaseHelper("vend_users.db");
            service.Start();

            var card1 = new ChipCard("00112233");
            var card2 = new ChipCard("aabbccdd");
            var card3 = new ChipCard("12ab34cd");

            var account1 = new User(){AssignedCard = card1, UserName = "Name1", Organization = "Org1", Phone = "+7(921)123-33-33"};
            var account2 = new User(){AssignedCard = card2, UserName = "Name2", Organization = "Org1", Phone = "+7(921)123-33-44"};

            var user1 = service.AddUser(account1);
            var user2 = service.AddUser(account2);

            service.SaveSale(user1.Id, 1, 20);

            var user = service.GetUser(card1);
            Console.WriteLine(user);
            user = service.GetUser(card2);
            Console.WriteLine(user);
            Console.ReadKey();
        }
    }
}
