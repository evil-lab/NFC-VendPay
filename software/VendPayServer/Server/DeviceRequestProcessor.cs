using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NLog;
using com.IntemsLab.Common;
using com.IntemsLab.Common.Model;
using com.IntemsLab.Communication.Protocol;
using com.IntemsLab.Server.Network;

namespace com.IntemsLab.Server
{
    enum CshlProtocolCommands
    {
        AmountReq = 0x01,
        SaleReq = 0x02,
        CancelReq = 0x03
    }

    enum CshlSessionError
    {
        UserNotRegistered = 0x03,
        SessionDuplication = 0x05,
        NotEnougthMoney = 0x02
    }

    class DeviceRequestProcessor
    {
        private readonly ProtocolListener _listener;
        private readonly DatabaseHelper _helper;

        private readonly RequestParser _parser;
        private readonly ResponseBuilder _builder;

        private readonly List<int> _activeUserIds;
        private readonly object _sessionLocker = new object();

        private readonly Logger _log;

        public DeviceRequestProcessor(int port, string databaseName)
        {
            _helper = new DatabaseHelper(databaseName);
            _listener = new ProtocolListener(IPAddress.Any, port);

            _parser = new RequestParser();
            _builder = new ResponseBuilder();

            //_log = LogManager.GetCurrentClassLogger();
            _log = LogManager.GetLogger("fileLogger");

            _activeUserIds = new List<int>();
        }

        public void Start()
        {
            _activeUserIds.Clear();

            _helper.Start();

            _listener.Request += OnRequest;
            _listener.Error += OnError;
            _listener.Start();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            var ex = e.GetException();
            _log.Error("Communication protocol error.\nMessage:{0}\nStack trace:\n{1}", ex.Message, ex.StackTrace);
        }

        private void OnRequest(object sender, ProtocolEventArgs e)
        {
            Console.WriteLine("Received command code: {0}", e.Response.CommandCode);
            
            var cmdCode = (CshlProtocolCommands) e.Request.CommandCode;
            switch (cmdCode)
            {
                case CshlProtocolCommands.AmountReq:
                    AmountReqProcessing(e);
                    break;

                case CshlProtocolCommands.SaleReq:
                    SaleReqProcessing(e);
                    break;

                case CshlProtocolCommands.CancelReq:
                    //e.Response = _vending.CancelSale(e.Request);
                    break;

                default:
                    Console.WriteLine("Invalid command received");
                    throw new ProtocolException(ProtocolError.InvalidCommand);
            }

        }

        // request process methods
        private void AmountReqProcessing(ProtocolEventArgs e)
        {
            var userInfo = _parser.GetUserInfo(e.Request);
            var card = new ChipCard(userInfo.Item2);

            lock (_sessionLocker)
            {
                var user = _helper.GetUser(card);
                _log.Debug("Card ID: {0}", card.CardId);
                if (user == null)
                {
                    e.Response = _builder.BuildError(e.Request, (int)CshlSessionError.UserNotRegistered);
                    return;
                }
                if (!_activeUserIds.Contains(user.Id))
                {
                    _activeUserIds.Add(user.Id);
                    e.Response = _builder.BuildUserInfo(e.Request, user);
                }
                else
                {
                    e.Response = _builder.BuildError(e.Request, (int) CshlSessionError.SessionDuplication);
                }
            }
        }

        private void SaleReqProcessing(ProtocolEventArgs e)
        {
            var vendInfo = _parser.GetVendInfo(e.Request);
            var userId = vendInfo.Item1;
            var cellId = vendInfo.Item2;
            var price = vendInfo.Item3;

            if (_activeUserIds.Contains(userId))
            {
                lock (_sessionLocker)
                {
                    _helper.SaveSale(userId, cellId, price);
                    _activeUserIds.Remove(userId);
                }
                e.Response = _builder.BuildSellInfo(e.Request);
            }
            else
            {
                e.Response = _builder.BuildError(e.Request, 0x09);
            }
        }
    }
}
