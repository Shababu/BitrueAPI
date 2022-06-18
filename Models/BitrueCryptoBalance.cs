using BinanceApiLibrary;
using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueCryptoBalance : ICryptoBalance
    {
        public string Asset { get; set; }
        public decimal Total { get; set; }
        public decimal Free { get; set; }
        public decimal Locked { get; set; }
        public decimal RubValue { get; set; }

        internal static BitrueCryptoBalance ConvertToCryptoBalance(BitrueAccountBalanceDeserialization balance)
        {
            BitrueCryptoBalance bitrueCrypto = new BitrueCryptoBalance() 
            {
                Asset = balance.Asset.ToUpper(),
                Total = Convert.ToDecimal(balance.Free.Replace(".", ",")) + Convert.ToDecimal(balance.Locked.Replace(".", ",")),
                Free = Convert.ToDecimal(balance.Free.Replace('.', ',')),
                Locked = Convert.ToDecimal(balance.Locked.Replace('.', ',')),
                RubValue = 0,
            };

            return bitrueCrypto;
        }
        internal static void CountRubValue(List<ICryptoBalance> balances)
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            List<IAssetStatus> allPairs = new BitrueMarketInfo().Get24HourStatOnAllAssets();
            decimal rubPrice = new BinanceMarketInfo().GetPrice("USDTRUB");

            foreach (var balance in balances)
            {
                if (balance.Asset == "RUB")
                {
                    balance.RubValue = Math.Round(balance.Total, 2);
                }
                else if (balance.Asset.Contains("USD"))
                {
                    balance.RubValue = Math.Round(balance.Total * rubPrice, 2);
                }
                else
                {
                    try
                    {
                        balance.RubValue = allPairs.Where(crypto => crypto.Symbol == balance.Asset + "USDT").First().LastPrice * balance.Total * rubPrice;
                        balance.RubValue = Math.Round(balance.RubValue, 2);
                    }
                    catch (Exception)
                    {
                        balance.RubValue = 0;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format($"Asset: {Asset}, Total: {Total}, Free: {Free}, Locked: {Locked}, Rub Value: {RubValue}");
        }
    }
}
