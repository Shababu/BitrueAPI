using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueWalletInfo : IWalletInfo
    {
        public string BaseUrl => "https://openapi.bitrue.com/";
        public string AccountInfoUrl => "api/v1/account?";

        public decimal GetAccountTotalBalance(List<ICryptoBalance> balances)
        {
            decimal totalBalance = 0;

            foreach (var balance in balances)
            {
                totalBalance += balance.RubValue;
            }

            return totalBalance;
        }
        public List<ICryptoBalance> GetWalletInfo(IExchangeUser user)
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            string response;

            string url = BaseUrl + AccountInfoUrl;
            string parameters = "recvWindow=10000&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            List<BitrueAccountBalanceDeserialization> rawInfo = BitrueAccountWalletDeserialization.DeserializeWalletInfo(response);
            List<ICryptoBalance> balances = new List<ICryptoBalance>();

            foreach (var info in rawInfo)
            {
                balances.Add(BitrueCryptoBalance.ConvertToCryptoBalance(info));
            }

            BitrueCryptoBalance.CountRubValue(balances);

            return balances;
        }
    }
}
