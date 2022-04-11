using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueAccountWalletDeserialization
    {
        public BitrueAccountBalanceDeserialization[] Balances { get; set; }

        public static List<BitrueAccountBalanceDeserialization> DeserializeWalletInfo(string jsonString)
        {
            BitrueAccountWalletDeserialization info = JsonConvert.DeserializeObject<BitrueAccountWalletDeserialization>(jsonString);
            List<BitrueAccountBalanceDeserialization> assets = info.Balances.Where(b => Convert.ToDouble(b.Free.Replace('.', ',')) > 0).ToList();
            return assets;
        }
    }
}
