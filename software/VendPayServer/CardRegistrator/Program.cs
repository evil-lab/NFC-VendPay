using System;
using System.IO;
using System.Threading;
using UserManager.Devices;

namespace CardRegistrator
{
    class Program
    {
        private static string fileName = "card_pool.dat";

        static void Main(string[] args)
        {
            var reader = new CardReader("COM5");
            reader.Init();
            reader.TagReceived += OnTagReceived;

            while (true)
                Thread.Sleep(500);
        }

        private static void OnTagReceived(object sender, CardReader.CardReaderEventArgs e)
        {
            var obj = (CardReader)sender;
            obj.StopListening();
            using (var fs = File.Open(fileName, FileMode.Append))
            {
                using (var writer = new StreamWriter(fs))
                {
                    var cardId = e.Card.CardId.Trim().ToLower();
                    Console.WriteLine("CARD: {0}", cardId);
                    writer.WriteLine(cardId);
                    writer.Flush();
                }
            }
            Console.WriteLine("Remove card & press any key");
            Console.ReadKey();
            obj.StartListening();
        }
    }
}
