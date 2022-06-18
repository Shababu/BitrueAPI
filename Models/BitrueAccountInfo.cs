using BitrueApiLibrary.Deserialization;
using Newtonsoft.Json;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueAccountInfo : IAccountInfo
    {
        public string BaseUrl => "https://openapi.bitrue.com/";
        public string TradeUrl => "api/v2/myTrades?";

        public List<IFilledTrade> GetTrades(IExchangeUser user, string symbol)
        {
            string response;
            BitrueMarketInfo binanceMarketInfo = new();

            string url = BaseUrl + TradeUrl;
            string parameters = "recvWindow=10000&symbol=" + symbol + "&limit=1000" + "&timestamp=" + binanceMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }          

            List<BitrueTradesDeserialization> trades = JsonConvert.DeserializeObject<List<BitrueTradesDeserialization>>(response);
            List<IFilledTrade> listOfTrades = new List<IFilledTrade>();
            foreach (var trade in trades)
            {
                BitrueFilledTrade filledTrade = BitrueFilledTrade.ConvertToFilledTrade(trade);
                if (filledTrade.IsBuyer)
                {
                    filledTrade.Side = Sides.BUY;
                }
                else
                {
                    filledTrade.Side = Sides.SELL;
                }
                listOfTrades.Add(filledTrade);
            }
            return listOfTrades;
        }
    }
}
