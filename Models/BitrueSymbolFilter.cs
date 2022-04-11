using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueSymbolFilter : ISymbolFilter
    {
        public string FilterType { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal PriceScale { get; set; }
        public decimal MinQty { get; set; }
        public decimal MinVal { get; set; }
    }
}
