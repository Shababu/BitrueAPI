namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueDepthDeserialization
    {
        public long LastUpdateId { get; set; }
        public List<List<object>>? Bids { get; set; }
        public List<List<object>>? Asks { get; set; }
    }
}
