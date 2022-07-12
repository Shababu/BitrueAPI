using System.Text;
using Newtonsoft.Json;

namespace BitrueApiLibrary.Deserialization
{
    internal class BitrueWithdrawalDeserialization
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string PayAmount { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public string Txid { get; set; }
        public string Confirmations { get; set; }
        public string Status { get; set; }
        public string TagType { get; set; }

        internal static List<BitrueWithdrawalDeserialization> DeserializeWithdrawal(string jsonString)
        {
            return JsonConvert.DeserializeObject<List<BitrueWithdrawalDeserialization>>(jsonString);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Id: ");
            sb.Append(Id);
            sb.Append($"\nSymbol: ");
            sb.Append(Symbol);
            sb.Append($"\nAmount: ");
            sb.Append(Amount);
            sb.Append($"\nFee: ");
            sb.Append(Fee);
            sb.Append($"\nPayAmount: ");
            sb.Append(PayAmount);
            sb.Append($"\nCreatedAt: ");
            sb.Append(CreatedAt);
            sb.Append($"\nUpdatedAt: ");
            sb.Append(UpdatedAt);
            sb.Append($"\nAddressFrom: ");
            sb.Append(AddressFrom);
            sb.Append($"\nAddressTo: ");
            sb.Append(AddressTo);
            sb.Append($"\nTxid: ");
            sb.Append(Txid);
            sb.Append($"\nConfirmations: ");
            sb.Append(Confirmations);
            sb.Append($"\nStatus: ");
            sb.Append(Status);
            sb.Append($"\nTagType: ");
            sb.Append(TagType);
            sb.Append($"\n");

            return sb.ToString();
        }
    }
}