using System;
using NLog;

namespace com.IntemsLab.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetCurrentClassLogger();

            try
            {
                var devProc = new DeviceRequestProcessor(6767, "vend.db");
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
