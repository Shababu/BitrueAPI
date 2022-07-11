using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueLimitOrder : ILimitOrder
    {
        public string Symbol { get; set; }
        public long OrderId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal ExecutedQty { get; set; }
        public string Status { get; set; }
        public string Side { get; set; }
        public DateTime Time { get; set; }

        public BitrueLimitOrder() { }

        public BitrueLimitOrder(string symbol, Sides side, decimal quantity, decimal price)
        {
            Symbol = symbol;
            Price = price;
            Quantity = quantity;
            if (side == Sides.BUY)
            {
                Side = "BUY";
            }
            else
            {
                Side = "SELL";
            }
        }

        internal static BitrueLimitOrder ConvertToLimitOrder(BitrueLimitOrderDeserialization order)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            BitrueLimitOrder limitOrder = new BitrueLimitOrder() {
                Symbol = order.Symbol,
                OrderId = Convert.ToInt64(order.OrderId),
                Price = order.Price is null ? decimal.Zero : Convert.ToDecimal(order.Price.Replace('.', ',')),
                Quantity = order.OrigQty is null ? decimal.Zero : Convert.ToDecimal(order.OrigQty.Replace('.', ',')),
                ExecutedQty = order.ExecutedQty is null ? decimal.Zero : Convert.ToDecimal(order.ExecutedQty.Replace('.', ',')),
                Status = order.Status,
                Side = order.Side,

                Time = order.TransactTime is null ? dateTime.AddMilliseconds(Convert.ToDouble(order.Time)).ToLocalTime() :
                                                dateTime.AddMilliseconds(Convert.ToDouble(order.TransactTime)).ToLocalTime(),
            };

            return limitOrder;
        }

        public override string ToString()
        {
            return string.Format($"Order Id: {OrderId}, Symbol: {Symbol}, Price: {Price}, Quantity: {Quantity}, ExecutedQty: {ExecutedQty}, Status: {Status}, Side: {Side}, Time: {Time}");
        }
    }
}
