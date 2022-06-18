using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueCandle : ICandle
    {
        public DateTime OpenTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }

        public BitrueCandle(DateTime openTime, double open, double high, double low, double close, double volume)
        {
            OpenTime = openTime;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }

        internal static List<ICandle> ConvertToCandle(BitrueCandlestickDeserialization candles)
        {
            List<ICandle> resultCandles = new List<ICandle>();

            foreach(var candle in candles.Data)
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(candle.I).ToLocalTime();
                double openPrice = Convert.ToDouble(candle.O.Replace('.', ','));
                double highPrice = Convert.ToDouble(candle.H.Replace('.', ','));
                double lowPrice = Convert.ToDouble(candle.L.Replace('.', ','));
                double closePrice = Convert.ToDouble(candle.C.Replace('.', ','));
                double volume = Convert.ToDouble(candle.V.Replace('.', ','));
                BitrueCandle bitrueCandle = new BitrueCandle(dateTime, openPrice, highPrice, lowPrice, closePrice, volume);
                resultCandles.Add(bitrueCandle);
            }

            return resultCandles;
        }

        public override string ToString()
        {
            return string.Format($"Open Time: {OpenTime}, Open: {Open}, High: {High}, Low: {Low}, Close: {Close}, Volume: {Volume}\n");
        }
    }
}
