using Newtonsoft.Json;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueAssetStatsDeserialization
    {
        public string? Symbol { get; set; }
        public string? PriceChange { get; set; }
        public string? PriceChangePercent { get; set; }
        public string? WeightedAvgPrice { get; set; }
        public string? PrevClosePrice { get; set; }
        public string? LastPrice { get; set; }
        public string? LastQty { get; set; }
        public string? OpenPrice { get; set; }
        public string? HighPrice { get; set; }
        public string? LowPrice { get; set; }
        public string? Volume { get; set; }
        public string? QuoteVolume { get; set; }

        internal static BitrueAssetStatsDeserialization DeserializeAssetStats(string json)
        {
            json = json.Trim(new char[] { '[', ']' });
            BitrueAssetStatsDeserialization? stats = JsonConvert.DeserializeObject<BitrueAssetStatsDeserialization>(json);
            if(stats != null)
            {
                return stats;
            }
            else return new BitrueAssetStatsDeserialization();
        }

        public override string ToString()
        {
            return string.Format($"PriceChange: {PriceChange}, PriceChangePercent: {PriceChangePercent}, WeightedAvgPrice: {WeightedAvgPrice}, " +
                $"LastPrice: {LastPrice}, LastQty: {LastQty}, OpenPrice: {OpenPrice}, HighPrice: {HighPrice}, LowPrice: {LowPrice}");
        }
    }
}
