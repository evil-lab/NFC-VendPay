using System;
using com.IntemsLab.Communication;
using com.IntemsLab.Communication.Protocol;

namespace com.IntemsLab.Server.Network
{
    class RequestParser
    {
        /// <summary>
        /// Retrive data from request frame
        /// </summary>
        /// <param name="frame">Request frame</param>
        /// <returns>Tuple with cardType card UID</returns>
        public Tuple<byte, string> GetUserInfo(RequestFrame frame)
        {
            Tuple<byte, string> result = null;
            try
            {
                var requestBytes = new Bytes(frame.Data);
                byte cardType = requestBytes.ReadByte();
                string uid = requestBytes.ReadNullTerminatedString();
                result = new Tuple<byte, string>(cardType, uid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame">Request frame</param>
        /// <returns>UserId, CellId, Product price</returns>
        public Tuple<int, int, int> GetVendInfo(RequestFrame frame)
        {
            Tuple<int, int, int> result = null;

            try
            {
                var requestBytes = new Bytes(frame.Data);
                int userId = requestBytes.ReadInt32();
                int cellId = requestBytes.ReadInt32();
                int price = requestBytes.ReadInt32();
                result = new Tuple<int, int, int>(userId, cellId, price);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }

        public Tuple<int> GetCancelInfo(RequestFrame frame)
        {
            return null;
        }
    }
}
