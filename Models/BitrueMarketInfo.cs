using BinanceApiLibrary;
using BitrueApiLibrary.Deserialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueMarketInfo : IMarketInfo
    {
        public decimal GetPrice(string pairSymbol = "XDCXRP") // Готово
        {
            string url = $"https://openapi.bitrue.com/api/v1/ticker/price?symbol={pairSymbol}";

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            string response;

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            decimal cryptoInfo = JsonConvert.DeserializeObject<Cryptocurrency>(response).Price;

            return cryptoInfo;
        }
        public string GetTimestamp() // Готово
        {
            return Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        }
        public IAssetStatus Get24HourStatOnAsset(string symbol) // Готово
        {
            string url = $"https://openapi.bitrue.com/api/v1/ticker/24hr?symbol={symbol}";
            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            string response;

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }
            return BitrueAssetStatus.ConvertToAssetStatus(BitrueAssetStatsDeserialization.DeserializeAssetStats(response));
        }
        public List<IDepth> GetDepth(string pairSymbol, int limit = 100)
        {
            string url = $"https://openapi.bitrue.com/api/v1/depth?symbol={pairSymbol}&limit={limit}";

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            string response;

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            List<string> info = response.Split(':').ToList();
            info.RemoveRange(0, 2);

            List<string> bids = info[0].Split(new string[] { ",[]"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> asks = info[1].Split(new string[] { ",[]" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            bids[0] = bids[0].Trim(']').Trim(']');
            asks[0] = asks[0].Trim(']').Trim(']');
            bids.RemoveAt(bids.Count - 1);
            asks.RemoveAt(asks.Count - 1);

            for (int i = 0; i < bids.Count; i++)
            {
                bids[i] = bids[i].Trim(new char[] { '[', ']', ',' });
                bids[i] = bids[i].Trim(new char[] { '[', ']', ',' });
            }

            for (int i = 0; i < asks.Count; i++)
            {
                asks[i] = asks[i].Trim(new char[] { '[', ']', ',' });
                asks[i] = asks[i].Trim(new char[] { '[', ']', ',' });
            }

            List<IDepth> depth = new List<IDepth>();

            foreach (var bid in bids)
            {
                string[] info2 = bid.Split(',');
                decimal bidPrice = Convert.ToDecimal(info2[0].Trim('\"').Replace('.', ','));
                decimal bidQnty = Convert.ToDecimal(info2[1].Trim('\"').Replace('.', ','));
                depth.Add(new BitrueDepth("bid", bidQnty, bidPrice));
            }

            foreach (var ask in asks)
            {
                string[] info2 = ask.Split(',');
                decimal askPrice = Convert.ToDecimal(info2[0].Trim('\"').Replace('.', ','));
                decimal askQnty = Convert.ToDecimal(info2[1].Trim('\"').Replace('.', ','));
                depth.Add(new BitrueDepth("ask", askQnty, askPrice));
            }

            return depth;
        }
        public List<ICandle> GetCandles(string symbol, string interval, int limit)
        {
            try
            {
                BinanceMarketInfo binanceMarketInfo = new BinanceMarketInfo();
                return binanceMarketInfo.GetCandles(symbol, interval, limit);
            }
            catch (Exception)
            {
                return new List<ICandle>();
            }
        }
        public List<IAssetStatus> Get24HourStatOnAllAssets()
        {
            string url = $"https://openapi.bitrue.com/api/v1/ticker/24hr";

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            string response;

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }
            List<IAssetStatus> assets = new List<IAssetStatus>();
            string[] assetsJson = response.Split(new string[] { "}," }, StringSplitOptions.RemoveEmptyEntries);

            assetsJson[0] = assetsJson[0].Trim('[');
            assetsJson[assetsJson.Length - 1] = assetsJson[assetsJson.Length - 1].Trim(']');
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
    }
}
