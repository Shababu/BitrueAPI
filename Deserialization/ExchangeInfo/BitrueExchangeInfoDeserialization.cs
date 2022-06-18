using Newtonsoft.Json;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueExchangeInfoDeserialization
    {
        public BitrueSymbolsInfoDeserialization[] Symbols { get; set; }

        internal static BitrueExchangeInfoDeserialization DeserializeExchangeInfo(string json)
        {
            BitrueExchangeInfoDeserialization symbols = JsonConvert.DeserializeObject<BitrueExchangeInfoDeserialization>(json);
            return symbols;
        }
    }
}
