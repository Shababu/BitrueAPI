using BitrueApiLibrary.Deserialization;
using System;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueAssetStatus : IAssetStatus // Готово
    {
        public string Symbol { get; set; }
        public decimal PriceChange { get; set; }
        public float PriceChangePercent { get; set; }
        public decimal WeightedAvgPrice { get; set; }
        public decimal PrevClosePrice { get; set; }
        public decimal LastPrice { get; set; }
        public decimal LastQty { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal Volume { get; set; }
        public decimal QuoteVolume { get; set; }

        internal static BitrueAssetStatus ConvertToAssetStatus(BitrueAssetStatsDeserialization stats)
        {
            BitrueAssetStatus assetStatus = new BitrueAssetStatus()
            {
                Symbol = stats.Symbol,
                PriceChange = stats.PriceChange != null ? Convert.ToDecimal(stats.PriceChange.Replace('.', ',')) : 0,
                PriceChangePercent = Convert.ToSingle(stats.PriceChangePercent.Replace('.', ',')),
                WeightedAvgPrice = stats.WeightedAvgPrice != null ? Convert.ToDecimal(stats.WeightedAvgPrice.Replace('.', ',')) : 0,
                PrevClosePrice = stats.PrevClosePrice != null ? Convert.ToDecimal(stats.PrevClosePrice.Replace('.', ',')) : 0,
                LastPrice = stats.LastPrice != null ? Convert.ToDecimal(stats.LastPrice.Replace('.', ',')) : 0,
                LastQty = stats.LastQty != null ? Convert.ToDecimal(stats.LastQty.Replace('.', ',')) : 0,
                OpenPrice = stats.OpenPrice != null ? Convert.ToDecimal(stats.OpenPrice.Replace('.', ',')) : 0,
                HighPrice = stats.HighPrice != null ? Convert.ToDecimal(stats.HighPrice.Replace('.', ',')) : 0,
                LowPrice = stats.LowPrice != null ? Convert.ToDecimal(stats.LowPrice.Replace('.', ',')) : 0,
                Volume = stats.Volume != null ? Convert.ToDecimal(stats.Volume.Replace('.', ',')) : 0,
                QuoteVolume = stats.QuoteVolume != null ? Convert.ToDecimal(stats.QuoteVolume.Replace('.', ',')) : 0,
            };

            return assetStatus;
        }

        public override string ToString()
        {
            return string.Format($"Symbol: {Symbol}\nPriceChange: {PriceChange}\nPriceChangePercent: {PriceChangePercent}\nWeightedAvgPrice: {WeightedAvgPrice}\n" +
                $"PrevClosePrice: {PrevClosePrice}\nLastPrice: {LastPrice}\nLastQty: {LastQty}\nOpenPrice: {OpenPrice}\nHighPrice: {HighPrice}\n" +
                $"LowPrice: {LowPrice}\nVolume: {Volume}\nQuoteVolume: { QuoteVolume}\n");
        }
    }
}
