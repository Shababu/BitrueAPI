using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueExchangeInfo : IExchangeInfo
    {
        public List<ISymbolInfo> ExchangeSymbolsInfo { get; set; }
        public List<ISimpleSymbol> ExchangeSymbols { get; set; }

        public BitrueExchangeInfo() 
        {
            ExchangeSymbolsInfo = new List<ISymbolInfo>();
            ExchangeSymbols = new List<ISimpleSymbol>();
        }
        internal static BitrueExchangeInfo ConvertToExchangeInfo(BitrueExchangeInfoDeserialization exchangeInfoRaw)
        {
            BitrueExchangeInfo exchangeInfo = new BitrueExchangeInfo();

            foreach (var symbol in exchangeInfoRaw.Symbols)
            {
                BitrueSymbolInfo symbolInfo = new BitrueSymbolInfo()
                {
                    Symbol = symbol.Symbol,
                    BaseAsset = symbol.BaseAsset,
                    QuoteAsset = symbol.QuoteAsset,
                    QuotePrecision = Convert.ToDecimal(symbol.QuotePrecision),
                    Filters = new List<ISymbolFilter>()                    
                };

                foreach(var filter in symbol.Filters)
                {
                    symbolInfo.Filters.Add(new BitrueSymbolFilter
                    {
                        FilterType = filter.FilterType,
                        MinPrice = filter.MinPrice is null ? 0 : Convert.ToDecimal(filter.MinPrice.Replace('.',',')),
                        MaxPrice = filter.MaxPrice is null ? 0 : Convert.ToDecimal(filter.MaxPrice.Replace('.', ',')),
                        PriceScale = filter.PriceScale is null ? 0 : Convert.ToDecimal(filter.PriceScale.Replace('.', ',')),
                        MinQty = filter.MinQty is null ? 0 : Convert.ToDecimal(filter.MinQty.Replace('.', ',')),
                        MinVal = filter.MinVal is null ? 0 : Convert.ToDecimal(filter.MinVal.Replace('.', ',')),
                    });
                }

                exchangeInfo.ExchangeSymbolsInfo.Add(symbolInfo);

                exchangeInfo.ExchangeSymbols.Add(new BitrueSimpleSymbol
                {
                    Symbol = symbol.Symbol.ToUpper(),
                    BaseAsset = symbol.BaseAsset.ToUpper(),
                    QuoteAsset = symbol.QuoteAsset.ToUpper()
                });
            }

            return exchangeInfo;
        }
        public IExchangeInfo GetExchangeInfo()
        {
            string url = "https://openapi.bitrue.com/api/v1/exchangeInfo";

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            string response;

            using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            BitrueExchangeInfoDeserialization symbols = BitrueExchangeInfoDeserialization.DeserializeExchangeInfo(response);
            return ConvertToExchangeInfo(symbols);
        }
    }
}
