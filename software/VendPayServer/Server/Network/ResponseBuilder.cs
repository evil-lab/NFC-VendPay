using com.IntemsLab.Communication;
using com.IntemsLab.Communication.Protocol;
using com.IntemsLab.Common.Model;

namespace com.IntemsLab.Server.Network
{
    class ResponseBuilder
    {
        public ResponseFrame BuildUserInfo(RequestFrame request, User user)
        {
            var respData = new byte[9];
            var respBytes = new Bytes(respData);
            respBytes.WriteInt32(user.Id); //user Id
            respBytes.WriteByte(1); //user type
            int amountInMCU = (int)user.Amount * 100; //переводим рубли в копейки
            respBytes.WriteInt32(amountInMCU); //user amount
            return new ResponseFrame(request.Address, request.FrameIndex, request.CommandCode, 0, respData);
        }

        public ResponseFrame BuildSellInfo(RequestFrame request)
        {
            var respData = new byte[4];
            var respBytes = new Bytes(respData);
            respBytes.WriteInt32(0x00); //STUB
            return new ResponseFrame(request.Address, request.FrameIndex, request.CommandCode, 0, respData);
        }

        public ResponseFrame BuildError(RequestFrame request, int p0)
        {
            throw new System.NotImplementedException();
        }
    }
}
