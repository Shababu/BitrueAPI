namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueSymbolsInfoDeserialization
    {
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public string QuoteAsset { get; set; }
        public string QuotePrecision { get; set; }

        public BitrueFiltersInfoDeserialization[] Filters { get; set; }
    }
}
