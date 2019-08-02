using System;
using NLog;
using com.IntemsLab.Common;
using System.Configuration;

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

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var usbDir = config.AppSettings.Settings["UsbDir"].Value;
                var archiveDir = config.AppSettings.Settings["ReportArchives"].Value;


                USBStickHandler usbHandler = new USBStickHandler(dbHelper, usbDir, archiveDir);
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
