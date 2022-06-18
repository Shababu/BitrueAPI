using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueTrader : ITrader
    {
        public ILimitOrder PlaceNewLimitOrder(IExchangeUser user, string symbol, Sides side, decimal quantity, decimal price) 
        {
            BitrueMarketInfo bitrueMarketInfo = new();
            string response;

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&side={side}&type=LIMIT&quantity={quantity.ToString().Replace(",", ".")}&price={price.ToString().Replace(",", ".")}&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            Thread.Sleep(1000);

            using(HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
                message.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.Send(message).Content.ReadAsStringAsync().Result;
            }

            BitrueLimitOrder order = BitrueLimitOrder.ConvertToLimitOrder(BitrueLimitOrderDeserialization.DeserializeLimitOrder(response));
            order.Side = side.ToString();
            order.Quantity = quantity;
            order.Price = price;
            order.Status = "NEW";
            return order;
        }
        public ILimitOrder PlaceNewMarketOrder(IExchangeUser user, string symbol, Sides side, decimal quantity)
        {
            BitrueMarketInfo binanceMarketInfo = new();
            string response;

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&side={side}&type=MARKET&quantity={quantity.ToString().Replace(",", ".")}&recvWindow=20000&timestamp=" + binanceMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url);
                message.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.Send(message).Content.ReadAsStringAsync().Result;
            }

            return BitrueLimitOrder.ConvertToLimitOrder(BitrueLimitOrderDeserialization.DeserializeLimitOrder(response));
        }
        public ILimitOrder CancelLimitOrder(IExchangeUser user, string symbol, string orderId) 
        {
            BitrueMarketInfo bitrueMarketInfo = new();
            ILimitOrder order = GetOrderInfo(user, orderId, symbol);

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&orderId={orderId}&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            string response;

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Delete, url);
                message.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.Send(message).Content.ReadAsStringAsync().Result;
            }

            order.Status = "CANCELED";

            return order;
        }
        public ILimitOrder GetOrderInfo(IExchangeUser user, string orderId, string symbol) 
        {
            BitrueMarketInfo bitrueMarketInfo = new();
            string response;

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&orderId={orderId}&recvWindow=10000&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
                message.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.Send(message).Content.ReadAsStringAsync().Result;
            }

            BitrueLimitOrder orderInfo = BitrueLimitOrder.ConvertToLimitOrder(BitrueLimitOrderDeserialization.DeserializeLimitOrder(response));

            return orderInfo;
        }
        public string GetOrderStatus(IExchangeUser user, SpotPosition position, string symbol) 
        {
            BitrueTrader trader = new();
            return trader.GetOrderInfo(user, position.OrderId, symbol).Status;
        }
        public List<ILimitOrder> GetOpenOrders(IExchangeUser user, string symbol) 
        {
            BitrueMarketInfo bitrueMarketInfo = new();
            string response;

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/openOrders?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&recvWindow=5000&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, url);
                message.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
                response = client.Send(message).Content.ReadAsStringAsync().Result;
            }

            List<BitrueLimitOrderDeserialization> rawOrders = BitrueLimitOrderDeserialization.DeserializeLimitOrders(response);
            List<ILimitOrder> orders = new();

            foreach (var rawOrder in rawOrders)
            {
                orders.Add(BitrueLimitOrder.ConvertToLimitOrder(rawOrder));
            }

            return orders;
        }
        public void PlaceInitialOrders(IExchangeUser user, List<SpotPosition> positions)
        {
            ILimitOrder newLimitOrder;

            foreach (var position in positions)
            {
                if (!position.IsBought && !position.IsBuyOrderPlaced)
                {
                    newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.BUY, position.Amount, position.BuyingPrice);
                    UpdateOrderStatusAndId(position, newLimitOrder);
                    position.IsBuyOrderPlaced = true;
                    position.InspectSpotPosition(); // Shoots an event that will inform user about order execution
                }

                else if (position.IsBought && !position.IsSellOrderPlaced)
                {
                    newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.SELL, position.Amount, position.SellingPrice);
                    UpdateOrderStatusAndId(position, newLimitOrder);
                    position.IsSellOrderPlaced = true;
                    position.InspectSpotPosition();
                }
            }
        }
        public void AutoTrade(IExchangeUser user, List<SpotPosition> positions, bool increaseAmount)
        {
            ILimitOrder newLimitOrder;
            var openOrders = GetOpenOrders(user, positions[0].Symbol);

            foreach (var position in positions)
            {
                var currentPosition = openOrders.Where(o => o.OrderId.ToString() == position.OrderId).FirstOrDefault();

                if (currentPosition == null)
                {
                    if (position.IsBought)
                    {
                        position.IsBought = position.IsBuyOrderPlaced = position.IsSellOrderPlaced = false;
                        position.InspectSpotPosition();
                        if (increaseAmount == true)
                        {
                            IncreaseAmount(position);
                        }
                        newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.BUY, position.Amount, position.BuyingPrice);
                        UpdateOrderStatusAndId(position, newLimitOrder);
                        position.IsBuyOrderPlaced = true;
                        position.InspectSpotPosition();
                    }
                    else
                    {
                        position.IsBought = true;
                        position.InspectSpotPosition();
                        newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.SELL, position.Amount, position.SellingPrice);
                        UpdateOrderStatusAndId(position, newLimitOrder);
                        position.IsSellOrderPlaced = true;
                        position.InspectSpotPosition();
                    }
                }
            }
        }
        private static void UpdateOrderStatusAndId(SpotPosition position, ILimitOrder newLimitOrder)
        {
            position.OrderId = newLimitOrder.OrderId.ToString();
            position.Status = newLimitOrder.Status;
        }
        private static void IncreaseAmount(SpotPosition position)
        {
            decimal newDollarAmount = (((position.Amount * position.SellingPrice) - (position.BuyingPrice * position.Amount)) * .994M) + (position.BuyingPrice * position.Amount);

            position.Amount = newDollarAmount / position.BuyingPrice;
            string middleValueInt = position.Amount.ToString();
            string middleValueDecimal = middleValueInt.Split(',')[1][..1];

            middleValueInt = middleValueInt.Split(',')[0];
            position.Amount = Convert.ToDecimal((middleValueInt + "," + middleValueDecimal));
        }
    }
}
