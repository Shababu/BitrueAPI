using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueDepth : IDepth
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public BitrueDepth(string name, decimal quantity, decimal price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
        }

        public BitrueDepth()
        {
            Name = "";
            Quantity = 0;
            Price = 0;
        }

        internal static List<IDepth> ConvertToDepth(BitrueDepthDeserialization rawDepth)
        {
            List<IDepth> depth = new List<IDepth>();

            foreach (var item in rawDepth.Bids)
            {
                decimal price = Convert.ToDecimal(item[0].ToString().Replace('.', ','));
                decimal amount = Convert.ToDecimal(item[1].ToString().Replace('.', ','));
                depth.Add(new BitrueDepth("Bid", amount, price));
            }

            foreach (var item in rawDepth.Asks)
            {
                decimal price = Convert.ToDecimal(item[0].ToString().Replace('.', ','));
                decimal amount = Convert.ToDecimal(item[1].ToString().Replace('.', ','));
                depth.Add(new BitrueDepth("Ask", amount, price));
            }
            return depth;
        }

        public override string ToString()
        {
            return string.Format($"{Name} Price: {Price}, Quantity: {Quantity}");
        }
    }
}
