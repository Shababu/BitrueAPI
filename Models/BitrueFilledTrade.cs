using System;
using TradingCommonTypes;
using BitrueApiLibrary.Deserialization;

namespace BitrueApiLibrary
{
    public class BitrueFilledTrade : IFilledTrade
    {
        public long OrderId { get; set; }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Qty { get; set; }
        public decimal QuoteQty { get; set; }
        public decimal Commission { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsBuyer { get; set; }
        public bool IsMaker { get; set; }
        public Sides Side { get; set; }

        internal static BitrueFilledTrade ConvertToFilledTrade(BitrueTradesDeserialization trade)
        {
            BitrueFilledTrade filledTrade = new BitrueFilledTrade()
            {
                OrderId = Convert.ToInt64(trade.OrderId),
                Symbol = trade.Symbol,
                Price = Convert.ToDecimal(trade.Price.Replace('.', ',')),
                Qty = Convert.ToDecimal(trade.Qty.Replace('.', ',')),
                QuoteQty = Convert.ToDecimal(trade.Price.Replace('.', ',')) * Convert.ToDecimal(trade.Qty.Replace('.', ',')),
                Commission = trade.Commission is null ? 0 : Convert.ToDecimal(trade.Commission.Replace('.', ',')),
                TimeStamp = BitrueApiUser.ConvertTimeStampToDateTime(Convert.ToDouble(trade.Time)),
                IsBuyer = Convert.ToBoolean(trade.IsBuyer),
                IsMaker = Convert.ToBoolean(trade.IsMaker),
            };

            return filledTrade;
        }

        public override string ToString()
        {
            return string.Format($"OrderId: {OrderId}, Symbol: {Symbol}, Price: {Price}, Qty: {Qty}, QuoteQty: {QuoteQty}, Commission: {Commission}, " +
                $"TimeStamp: {TimeStamp}, IsBuyer: {IsBuyer}, IsMaker: {IsMaker}");
        }
    }
}
