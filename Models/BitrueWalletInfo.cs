using BitrueApiLibrary.Deserialization;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            string response;
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();

            string url = BaseUrl + AccountInfoUrl;
            string parameters = "recvWindow=10000&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HTTPrequest.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
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
