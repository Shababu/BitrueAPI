using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueWalletInfo : IWalletInfo
    {
        public string BaseUrl => "https://openapi.bitrue.com/";
        public string AccountInfoUrl => "api/v1/account?";

        public List<ICryptoBalance> GetWalletInfo(IExchangeUser user)
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            string response;

            string url = BaseUrl + AccountInfoUrl;
            string parameters = "recvWindow=10000&timestamp=" + bitrueMarketInfo.GetTimestamp(DateTime.UtcNow);
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
        public decimal GetAccountTotalBalance(List<ICryptoBalance> balances)
        {
            decimal totalBalance = 0;

            foreach (var balance in balances)
            {
                totalBalance += balance.RubValue;
            }

            return totalBalance;
        }
        public List<BitrueDeposit> GetRecentDeposits(IExchangeUser user, string coin, DateTime startTime = default(DateTime), DateTime endTime = default(DateTime))
        {
            string url = BaseUrl + "api/v1/deposit/history?";
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();

            string response;

            string parameters = "recvWindow=10000&timestamp=" + bitrueMarketInfo.GetTimestamp(DateTime.UtcNow);

            if (startTime != default(DateTime))
            {
                parameters += $"&startTime={bitrueMarketInfo.GetTimestamp(startTime)}";

                if (endTime == default(DateTime) || endTime < startTime)
                {
                    endTime = startTime.AddDays(90);
                }
                else
                {
                    if (endTime.Subtract(startTime).Days > 90)
                    {
                        endTime = startTime.AddDays(90);
                    }
                }
                parameters += $"&endTime={bitrueMarketInfo.GetTimestamp(endTime)}";
            }
            parameters += $"&coin={coin}&status=1";

            url += parameters + $"&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            response = response.Substring(response.IndexOf('['));
            response = response.Trim('}');

            List<BitrueDepositDeserialization> rawDeposits = BitrueDepositDeserialization.DeserializeDeposit(response);
            List<BitrueDeposit> result = new List<BitrueDeposit>();

            foreach (var rawDeposit in rawDeposits)
            {
                result.Add(BitrueDeposit.ConvertToDeposit(rawDeposit));
            }

            return result;
        }
    }
}
