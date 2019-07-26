using com.IntemsLab.Communication.Protocol;

namespace UserManager.Server
{
    internal interface IVendProtocol
    {
        ResponseFrame GetUserInfo(RequestFrame request);
        ResponseFrame MakeSale(RequestFrame request);
        ResponseFrame CancelSale(RequestFrame request);
    }
}