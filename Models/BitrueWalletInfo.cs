using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueWalletInfo : IWalletInfo
    {
        private string baseUrl = "https://openapi.bitrue.com/";

        public List<ICryptoBalance> GetWalletInfo(IExchangeUser user)
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            string response;

            string url = baseUrl + "api/v1/account?";
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
        public List<IDeposit> GetRecentDeposits(IExchangeUser user, string coin, DateTime startTime = default(DateTime), DateTime endTime = default(DateTime))
        {
            string url = baseUrl + "api/v1/deposit/history?";
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
            List<IDeposit> result = new List<IDeposit>();

            foreach (var rawDeposit in rawDeposits)
            {
                result.Add(BitrueDeposit.ConvertToDeposit(rawDeposit));
            }

            return result;
        }
        public List<IWithdrawal> GetRecentWithdrawals(IExchangeUser user, string coin, DateTime startTime = default(DateTime), DateTime endTime = default(DateTime))
        {
            string url = baseUrl + "api/v1/withdraw/history?";
            BitrueMarketInfo binanceMarketInfo = new BitrueMarketInfo();
            string response;

            string parameters = "recvWindow=10000&timestamp=" + binanceMarketInfo.GetTimestamp(DateTime.UtcNow);
            parameters += $"&coin={coin}&status=5";
          
            if (startTime != default(DateTime))
            {
                parameters += $"&startTime={binanceMarketInfo.GetTimestamp(startTime)}";

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
                parameters += $"&endTime={binanceMarketInfo.GetTimestamp(endTime)}";
            }

            url += parameters + "&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }

            response = response.Substring(response.IndexOf('['));
            response = response.Trim('}');

            List<BitrueWithdrawalDeserialization> rawWithdrawals = BitrueWithdrawalDeserialization.DeserializeWithdrawal(response);
            List<IWithdrawal> result = new List<IWithdrawal>();

            foreach (var rawWithdrawal in rawWithdrawals)
            {
                result.Add(BitrueWithdrawal.ConvertToWithdrawal(rawWithdrawal));
            }

            return result;
        }
    }
}
