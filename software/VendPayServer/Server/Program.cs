using System;
using NLog;
using com.IntemsLab.Common;

namespace com.IntemsLab.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetCurrentClassLogger();

            try
            {
                DatabaseHelper dbHelper = new DatabaseHelper("vend.db");
                dbHelper.Start();

                USBStickHandler usbHandler = new USBStickHandler(dbHelper);
                DeviceRequestProcessor devProc = new DeviceRequestProcessor(6767, dbHelper, usbHandler);

                usbHandler.Start();
                devProc.Start();

                while (true)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                log.Fatal(e.Message);
                log.Fatal(e.StackTrace);
            }
        }
    }
}
