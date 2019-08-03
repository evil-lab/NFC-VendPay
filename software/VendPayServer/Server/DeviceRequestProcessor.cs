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
        SaleReq   = 0x02,
        CancelReq = 0x03
    }

    enum CshlSessionError
    {
        UserNotRegistered  = 0x03,
        SessionDuplication = 0x05,
        NotEnougthMoney    = 0x02
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

        public DeviceRequestProcessor(int port, DatabaseHelper dbHelper, USBStickHandler handler)
        {
            _log = LogManager.GetLogger("fileLogger");

            handler.Configuring += OnConfiguring;
            handler.Configured += OnConfigured;
            _helper = dbHelper;

            _activeUserIds = new List<int>();

            _listener = new ProtocolListener(IPAddress.Any, port);
            _parser = new RequestParser();
            _builder = new ResponseBuilder();
        }

        public void Start()
        {
            _activeUserIds.Clear();

            _listener.Request += OnRequest;
            _listener.Error += OnError;
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Request -= OnRequest;
            _listener.Error -= OnError;
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

        private void OnConfiguring(object sender, EventArgs e)
        {
            _log.Debug("Configuring event started");
            Stop();
        }

        private void OnConfigured(object sender, EventArgs e)
        {
            _log.Debug("Configurated event");
            Start();
        }

        // request process methods
        private void AmountReqProcessing(ProtocolEventArgs e)
        {
            var userInfo = _parser.GetUserInfo(e.Request);
            var card = new ChipCard(userInfo.Item2);

            lock (_sessionLocker)
            {
                var user = _helper.GetUser(card);
                if(user != null)
                {
                    _log.Debug("User ID:{0}  CardID:{1}", user.Id, user.AssignedCard);
                    if (!_activeUserIds.Contains(user.Id))
                    {
                        _activeUserIds.Add(user.Id);
                        e.Response = _builder.BuildUserInfo(e.Request, user);
                    }
                    else
                    {
                        e.Response = _builder.BuildError(e.Request, (int)CshlSessionError.SessionDuplication);
                    }
                }
                else 
                {
                    _log.Debug("CardID:{0} not assigned.", card.CardId);

                    e.Response = _builder.BuildError(e.Request, (int)CshlSessionError.UserNotRegistered);
                    return;
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
