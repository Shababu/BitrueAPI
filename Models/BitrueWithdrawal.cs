using System.Text;
using BitrueApiLibrary.Deserialization;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    internal class BitrueWithdrawal : IWithdrawal
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionFee { get; set; }
        public string Coin { get; set; }
        public int Status { get; set; }
        public string Address { get; set; }
        public string TxId { get; set; }
        public DateTime ApplyTime { get; set; }
        public decimal PayAmount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string AddressFrom { get; set; }
        public int Confirmations { get; set; }
        public string TagType { get; set; }


        internal static BitrueWithdrawal ConvertToWithdrawal(BitrueWithdrawalDeserialization withdrawalRaw)
        {
            BitrueWithdrawal withdrawal = new BitrueWithdrawal()
            {
                Id = withdrawalRaw.Id,
                Amount = Convert.ToDecimal(withdrawalRaw.Amount.Replace('.', ',')),
                TransactionFee = withdrawalRaw.Fee,
                Coin = withdrawalRaw.Symbol.ToUpper(),
                Status = Convert.ToInt32(withdrawalRaw.Status),
                Address = withdrawalRaw.AddressTo,
                TxId = withdrawalRaw.Txid,
                ApplyTime = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToInt64(withdrawalRaw.CreatedAt)),
                PayAmount = Convert.ToDecimal(withdrawalRaw.PayAmount.Replace('.', ',')),
                UpdatedAt = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToInt64(withdrawalRaw.UpdatedAt)),
                AddressFrom = withdrawalRaw.AddressFrom,
                Confirmations = Convert.ToInt32(withdrawalRaw.Confirmations),
                TagType = withdrawalRaw.TagType,
            };

            return withdrawal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Id: ");
            sb.Append(Id);
            sb.Append($"\nAmount: ");
            sb.Append(Amount);
            sb.Append($"\nTransactionFee: ");
            sb.Append(TransactionFee);
            sb.Append($"\nCoin: ");
            sb.Append(Coin);
            sb.Append($"\nStatus: ");
            sb.Append(Status);
            sb.Append($"\nAddress: ");
            sb.Append(Address);
            sb.Append($"\nTxId: ");
            sb.Append(TxId);
            sb.Append($"\nApplyTime: ");
            sb.Append(ApplyTime);
            sb.Append($"\nPayAmount: ");
            sb.Append(PayAmount);
            sb.Append($"\nUpdatedAt: ");
            sb.Append(UpdatedAt);
            sb.Append($"\nAddressFrom: ");
            sb.Append(AddressFrom);
            sb.Append($"\nConfirmations: ");
            sb.Append(Confirmations);
            sb.Append($"\nTagType: ");
            sb.Append(TagType);
            sb.Append("\n");

            return sb.ToString();
        }

    }
}
