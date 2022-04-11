using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueTrader : ITrader
    {
        public void AutoTrade(IExchangeUser user, List<SpotPosition> positions, bool increaseAmount)
        {
            ILimitOrder newLimitOrder;
            try
            {
                foreach (var position in positions)
                {
                    if (!position.IsBought && !position.IsBuyOrderPlaced)
                    {
                        newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.BUY, position.Amount, position.BuyingPrice);
                        UpdateOrderStatusAndId(position, newLimitOrder);

                        position.IsBuyOrderPlaced = true;
                        position.InspectSpotPosition();
                    }

                    else if (!position.IsBought && position.IsBuyOrderPlaced)
                    {
                        position.Status = GetOrderInfo(user, position.OrderId, position.Symbol).Status;
                        if (position.Status == "FILLED")
                        {
                            position.IsBought = true;
                            position.InspectSpotPosition();

                            newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.SELL, position.Amount * 0.997m, position.SellingPrice);
                            UpdateOrderStatusAndId(position, newLimitOrder);
                            position.IsSellOrderPlaced = true;
                            position.InspectSpotPosition();
                            continue;
                        }
                    }

                    else if (position.IsBought && !position.IsSellOrderPlaced)
                    {
                        newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.SELL, position.Amount, position.SellingPrice);
                        UpdateOrderStatusAndId(position, newLimitOrder);
                        position.IsSellOrderPlaced = true;
                        position.InspectSpotPosition();
                    }

                    if (position.IsBought && position.IsSellOrderPlaced)
                    {
                        position.Status = GetOrderInfo(user, position.OrderId, position.Symbol).Status;
                        if (position.Status == "FILLED")
                        {
                            if (increaseAmount == true)
                            {
                                IncreaseAmount(position);
                            }
                            position.IsBought = position.IsBuyOrderPlaced = position.IsSellOrderPlaced = false;
                            position.InspectSpotPosition();
                            newLimitOrder = PlaceNewLimitOrder(user, position.Symbol, Sides.BUY, position.Amount, position.BuyingPrice);
                            UpdateOrderStatusAndId(position, newLimitOrder);

                            position.IsBuyOrderPlaced = true;
                            position.InspectSpotPosition();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public void UpdateOrderStatusAndId(SpotPosition position, ILimitOrder newLimitOrder)
        {
            position.OrderId = newLimitOrder.OrderId.ToString();
            position.Status = newLimitOrder.Status;
        }
        public ILimitOrder PlaceNewLimitOrder(IExchangeUser user, string symbol, Sides side, decimal quantity, decimal price) // Готово
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&side={side}&type=LIMIT&quantity={quantity.ToString().Replace(",", ".")}&price={price.ToString().Replace(",", ".")}&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            string response;

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HTTPrequest.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
            HTTPrequest.Method = "POST";

            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            BitrueLimitOrder order = BitrueLimitOrder.ConvertToLimitOrder(BitrueLimitOrderDeserialization.DeserializeLimitOrder(response));
            order.Symbol = symbol;
            order.Side = side.ToString();
            order.Quantity = quantity;
            order.Price = price;
            order.Status = "NEW";
            return order;
        }
        public ILimitOrder PlaceNewMarketOrder(IExchangeUser user, string symbol, Sides side, decimal quantity)
        {
            BitrueMarketInfo binanceMarketInfo = new BitrueMarketInfo();
            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&side={side}&type=MARKET&quantity={quantity.ToString().Replace(",", ".")}&recvWindow=20000&timestamp=" + binanceMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            string response;

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HTTPrequest.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
            HTTPrequest.Method = "POST";
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            return BitrueLimitOrder.ConvertToLimitOrder(BitrueLimitOrderDeserialization.DeserializeLimitOrder(response));
        }
        public ILimitOrder CancelLimitOrder(IExchangeUser user, string symbol, string orderId) // Готово
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            ILimitOrder order = GetOrderInfo(user, orderId, symbol);

            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&orderId={orderId}&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            string response;

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HTTPrequest.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
            HTTPrequest.Method = "DELETE";
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            order.Status = "CANCELED";

            return order;
        }
        public List<ILimitOrder> GetOpenOrders(IExchangeUser user, string symbol) // Готово
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/openOrders?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&recvWindow=5000&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            string response;

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HTTPrequest.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
            HTTPrequest.Method = "GET";
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }

            List<BitrueLimitOrderDeserialization> rawOrders = BitrueLimitOrderDeserialization.DeserializeLimitOrders(response);
            List<ILimitOrder> orders = new List<ILimitOrder>();

            foreach (var rawOrder in rawOrders)
            {
                orders.Add(BitrueLimitOrder.ConvertToLimitOrder(rawOrder));
            }

            return orders;
        }
        public ILimitOrder GetOrderInfo(IExchangeUser user, string orderId, string symbol)
        {
            BitrueMarketInfo bitrueMarketInfo = new BitrueMarketInfo();
            string baseUrl = "https://openapi.bitrue.com/";
            string orderUrl = "api/v1/order?";
            string url = baseUrl + orderUrl;
            string parameters = $"symbol={symbol}&orderId={orderId}&recvWindow=10000&timestamp=" + bitrueMarketInfo.GetTimestamp();
            url += parameters + "&signature=" + user.Sign(parameters);

            string response;

            HttpWebRequest HTTPrequest = (HttpWebRequest)WebRequest.Create(url);
            HTTPrequest.Headers.Add("X-MBX-APIKEY", user.ApiPublicKey);
            HttpWebResponse HTTPresponse = (HttpWebResponse)HTTPrequest.GetResponse();

            using (StreamReader reader = new StreamReader(HTTPresponse.GetResponseStream()))
            {
                response = reader.ReadToEnd();
            }
            BitrueLimitOrder orderInfo = BitrueLimitOrder.ConvertToLimitOrder(BitrueLimitOrderDeserialization.DeserializeLimitOrder(response));
            return orderInfo;
        }
        public void IncreaseAmount(SpotPosition position)
        {
            decimal newDollarAmount = (((position.Amount * position.SellingPrice) - (position.BuyingPrice * position.Amount)) * .994M) + (position.BuyingPrice * position.Amount);

            position.Amount = newDollarAmount / position.BuyingPrice;
            string middleValueInt = position.Amount.ToString();
            string middleValueDecimal = middleValueInt.Split(',')[1].Substring(0, 1);
            middleValueInt = middleValueInt.Split(',')[0];
            position.Amount = Convert.ToDecimal((middleValueInt + "," + middleValueDecimal));
        }
        public string GetOrderStatus(IExchangeUser user, SpotPosition position, string symbol)
        {
            BitrueTrader trader = new BitrueTrader();
            return trader.GetOrderInfo(user, position.OrderId, symbol).Status;
        }
    }
}
