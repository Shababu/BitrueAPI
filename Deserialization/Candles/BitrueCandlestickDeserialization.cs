using Newtonsoft.Json;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueCandlestickDeserialization
    {
        public string Symbol { get; set; }
        public string Scale { get; set; }
        public List<Data> Data { get; set; }


        internal static BitrueCandlestickDeserialization DeserializeCandlestick(string jsonString)
        {
            BitrueCandlestickDeserialization result = new BitrueCandlestickDeserialization();
            return JsonConvert.DeserializeObject<BitrueCandlestickDeserialization>(jsonString);
        }
    }

    internal class Data
    {
        public double I { get; set; }
        public string A { get; set; }
        public string V { get; set; }
        public string C { get; set; }
        public string H { get; set; }
        public string L { get; set; }
        public string O { get; set; }
    }
}
