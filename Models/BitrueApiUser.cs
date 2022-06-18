using System.Security.Cryptography;
using System.Text;
using TradingCommonTypes;


namespace BitrueApiLibrary
{
    public class BitrueApiUser : IExchangeUser
    {
        public string ApiPublicKey { get; }
        public string ApiPrivateKey { get; }

        private readonly HMAC _hmac;

        public BitrueApiUser(string publicApiKey, string privateApiKey)
        {
            ApiPublicKey = publicApiKey;
            ApiPrivateKey = privateApiKey;
            _hmac = new HMACSHA256(Encoding.UTF8.GetBytes(privateApiKey));
        }

        public string Sign(string totalParams)
        {
            if (_hmac == null)
            {
                throw new InvalidOperationException($"{nameof(BitrueApiUser)}.{nameof(Sign)} requires the user's API secret.");
            }
            byte[] hash;
            hash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(totalParams));

            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        internal static DateTime ConvertTimeStampToDateTime(double timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTime.AddMilliseconds(timestamp).ToLocalTime();
        }
    }
}
