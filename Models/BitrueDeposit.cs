using BitrueApiLibrary.Deserialization;
using System.Text;
using TradingCommonTypes;

namespace BitrueApiLibrary
{
    public class BitrueDeposit : IDeposit
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string TransactionFee { get; set; }
        public string Coin { get; set; }
        public int Status { get; set; }
        public string Address { get; set; }
        public string TxId { get; set; }
        public DateTime ApplyTime { get; set; }

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
            sb.Append($"\n");

            return sb.ToString();
        }

        internal static BitrueDeposit ConvertToDeposit(BitrueDepositDeserialization depositRaw)
        {
            BitrueDeposit deposit = new BitrueDeposit()
            {
                Id = depositRaw.Id,
                Coin = depositRaw.Symbol,
                Amount = Convert.ToDecimal(depositRaw.Amount.Replace('.', ',')),
                TransactionFee = depositRaw.Fee,
                Address = depositRaw.AddressFrom,
                TxId = depositRaw.Txid,
                ApplyTime =  new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToInt64(depositRaw.CreatedAt)),
                Status = Convert.ToInt32(depositRaw.Status),
            };

            return deposit;
        }
    }
}
