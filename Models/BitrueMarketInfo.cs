using BitrueApiLibrary.Deserialization;
using Newtonsoft.Json;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueMarketInfo : IMarketInfo
    {
        public IAssetStatus Get24HourStatOnAsset(string symbol) 
        {
            string url = $"https://openapi.bitrue.com/api/v1/ticker/24hr?symbol={symbol}";
            string response;

            using (HttpClient client = new())
            {
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            return BitrueAssetStatus.ConvertToAssetStatus(BitrueAssetStatsDeserialization.DeserializeAssetStats(response));
        }
        public decimal GetPrice(string pairSymbol)
        {
            string url = $"https://openapi.bitrue.com/api/v1/ticker/price?symbol={pairSymbol}";
            string response;

            using (HttpClient client = new())
            {
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            var currentPrice = JsonConvert.DeserializeObject<Cryptocurrency>(response);

            if (currentPrice != null)
            {
                return currentPrice.Price;
            }
            else return 0;
        }
        public List<IAssetStatus> Get24HourStatOnAllAssets()
        {
            string url = $"https://openapi.bitrue.com/api/v1/ticker/24hr";
            string response;

            using(HttpClient client = new())
            {
                response= client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            List<IAssetStatus> assets = new();
            string[] assetsJson = response.Split(new string[] { "}," }, StringSplitOptions.RemoveEmptyEntries);

            assetsJson[0] = assetsJson[0].Trim('[');
            assetsJson[^1] = assetsJson[^1].Trim(']');

            for (int i = 0; i < assetsJson.Length - 1; i++)
            {
                assetsJson[i] = assetsJson[i] + '}';
            }

            foreach (var asset in assetsJson)
            {
                assets.Add(BitrueAssetStatus.ConvertToAssetStatus(BitrueAssetStatsDeserialization.DeserializeAssetStats(asset)));
            }

            return assets;
        }
        public List<IDepth> GetDepth(string pairSymbol, int limit = 100)
        {
            string url = $"https://openapi.bitrue.com/api/v1/depth?symbol={pairSymbol}&limit={limit}";
            string response;

            using(HttpClient client = new())
            {
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            var depth = JsonConvert.DeserializeObject<BitrueDepthDeserialization>(response);
            return BitrueDepth.ConvertToDepth(depth);
        }
        public List<ICandle> GetCandles(string symbol, string interval, int limit)
        {
            string url = $"https://openapi.bitrue.com/api/v1/market/kline?symbol={symbol}&interval={interval}&limit={limit}";
            string response;

            using (HttpClient client = new())
            {
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            BitrueCandlestickDeserialization candles = BitrueCandlestickDeserialization.DeserializeCandlestick(response);
            return BitrueCandle.ConvertToCandle(candles);                       
        }
        internal string GetTimestamp(DateTime dateTime)
        {
            return Math.Round((dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        }
    }
}
