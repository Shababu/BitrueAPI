using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueLimitOrderDeserialization
    {
        public string Symbol { get; set; }
        public string OrderId { get; set; }
        public string Price { get; set; }
        public string OrigQty { get; set; }
        public string ExecutedQty { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Side { get; set; }
        public string StopPrice { get; set; }
        public string TransactTime { get; set; }
        public string Time { get; set; }
        public string ConvertedTime { get; set; }


        public static BitrueLimitOrderDeserialization DeserializeLimitOrder(string jsonString)
        {
            BitrueLimitOrderDeserialization order = JsonConvert.DeserializeObject<BitrueLimitOrderDeserialization>(jsonString);
            return order;
        }

        public static List<BitrueLimitOrderDeserialization> DeserializeLimitOrders(string jsonString)
        {
            object[] trades = JsonConvert.DeserializeObject<object[]>(jsonString);
            List<BitrueLimitOrderDeserialization> orders = new List<BitrueLimitOrderDeserialization>();
            foreach(var order in trades)
            {
                BitrueLimitOrderDeserialization limitOrder = JsonConvert.DeserializeObject<BitrueLimitOrderDeserialization>(order.ToString());
                limitOrder.ConvertedTime = ConvertOrderTime(limitOrder.Time);
                orders.Add(limitOrder);
            }
            return orders;
        }

        public override string ToString()
        {
            return $"{ConvertedTime}\n{Symbol}\n{OrderId}\n{Price}\n{OrigQty}\n{ExecutedQty}\n{Status}\n{Type}\n{Side}\n{StopPrice}\n";
        }

        public static string ConvertOrderTime(string timeString)
        {
            return (new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(Convert.ToInt64(timeString))).ToString();
        }
    }
}
