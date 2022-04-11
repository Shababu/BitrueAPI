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

        public override string ToString()
        {
            return string.Format($"{Name} Price: {Price}, Quantity: {Quantity}");
        }
    }
}
