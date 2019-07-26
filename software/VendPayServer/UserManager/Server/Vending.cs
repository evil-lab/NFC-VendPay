using System;
using System.Collections.Generic;
using com.IntemsLab.Common;
using com.IntemsLab.Communication;
using com.IntemsLab.Communication.Protocol;
using com.IntemsLab.Common.Model;

namespace UserManager.Server
{
    internal class Vending : IVendProtocol
    {
        private readonly FileStorageHelper _storage;

        public Vending(FileStorageHelper storage)
        {
            _storage = storage;
        }

        public ResponseFrame GetUserInfo(RequestFrame request)
        {
            Console.WriteLine("Get balance received:");

            var requestBytes = new Bytes(request.Data);
            byte cardType = requestBytes.ReadByte();
            string uid = requestBytes.ReadNullTerminatedString();

            Console.WriteLine("CardType: {0}; Uid: {1}", cardType, uid);

            var respData = new byte[0];
            if (_storage != null)
            {
                var card = new ChipCard(uid);
                User user = _storage.GetUser(card);
                if (user != null)
                {
                    Console.WriteLine("User: {0}| Amount: {1}", user.UserName, user.Amount);

                    respData = new byte[9];
                    var respBytes = new Bytes(respData);
                    respBytes.WriteInt32(user.Id); //user Id
                    respBytes.WriteByte(1); //user type
                    int amountInMCU = (int) user.Amount*100; //переводим рубли в копейки
                    respBytes.WriteInt32(amountInMCU); //user amount
                }
                else
                {
                    Console.WriteLine("Card: {0} not registered", uid);
                    //return CARD_NOT_REGISTERED
                    return new ResponseFrame(request.Address, request.FrameIndex, request.CommandCode, 0xFC, respData);
                }
            }
            //return user balance
            return new ResponseFrame(request.Address, request.FrameIndex, request.CommandCode, 0, respData);
        }

        public ResponseFrame MakeSale(RequestFrame request)
        {
            Console.WriteLine("Make sale received:");

            var requestBytes = new Bytes(request.Data);
            int userId = requestBytes.ReadInt32();
            int cellId = requestBytes.ReadInt32();
            int price = requestBytes.ReadInt32();

            Console.WriteLine("UserId: {0}; CellId: {1}; Price: {2}", userId, cellId, price);

            // списание средств
            KeyValuePair<ChipCard, User> pair = _storage.GetAccountById(userId);
            var card = pair.Key;
            var user = pair.Value;
            if (card != null)
            {
                var amountDec = price/100; //Переводим копейки в рубли
                _storage.SaveSale(user.Id, cellId, amountDec);
            }
            else
            {
                Console.WriteLine("Can't find UserId in CardStorage");
            }

            var responseData = new byte[4];
            var responseBytes = new Bytes(responseData);
            responseBytes.WriteInt32(1234545);

            return new ResponseFrame(request.Address, request.FrameIndex, request.CommandCode, 0, responseData);
        }

        public ResponseFrame CancelSale(RequestFrame request)
        {
            var requestBytes = new Bytes(request.Data);
            int saleId = requestBytes.ReadInt32();

            Console.WriteLine("SaleId: {0}", saleId);
            return new ResponseFrame(request.Address, request.FrameIndex, request.CommandCode, 0, new byte[0]);
        }
    }
}