using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueTradesDeserialization
    {
        public string Symbol { get; set; }
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Price { get; set; }
        public string Qty { get; set; }
        public string Commission { get; set; }
        public string Time { get; set; }
        public string IsBuyer { get; set; }
        public string IsMaker { get; set; }


        public static List<BitrueTradesDeserialization> DeserializeLimitOrders(string jsonString)
        {
            object[] trades = JsonConvert.DeserializeObject<object[]>(jsonString);
            List<BitrueTradesDeserialization> orders = new List<BitrueTradesDeserialization>();
            return orders;
        }

        public static string ConvertOrderTime(string timeString)
        {
            return (new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(Convert.ToInt64(timeString))).ToString();
        }
    }
}
