using System;
using System.Collections.Generic;
using System.IO;

namespace com.IntemsLab.Common.Model
{
    class VMCPriceList
    {
        private const string FileName = "pricelist.txt";
        private readonly Dictionary<int, uint> _priceList;

        public VMCPriceList()
        {
            _priceList = new Dictionary<int, uint>();
        }

        public void Init()
        {
            using (var fs = File.Open(FileName, FileMode.Open))
            {
                var reader = new StreamReader(fs);
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if (!String.IsNullOrEmpty(str))
                    {
                        string[] pair = str.Split(':');
                        int position;
                        if (int.TryParse(pair[0], out position))
                        {
                            uint price;
                            if (uint.TryParse(pair[1], out price))
                                _priceList.Add(position, price);
                        }
                    }
                }
            }
        }
    }
}
