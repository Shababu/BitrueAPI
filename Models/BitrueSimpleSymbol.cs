using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueSimpleSymbol : ISimpleSymbol
    {
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
    }
}
