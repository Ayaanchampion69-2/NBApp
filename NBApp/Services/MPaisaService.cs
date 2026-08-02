using System.Security.Cryptography;
using System.Text;

namespace NBApp.Services
{
    public class MPaisaService(IConfiguration config)
    {
        private readonly string _merchantId = config["MPaisa:MerchantId"]!;
        private readonly string _hashKey = config["MPaisa:HashKey"]!;
        private readonly string _gatewayUrl = config["MPaisa:GatewayUrl"]!;

        public string BuildCheckoutUrl(int orderRef, decimal amount, string returnUrl)
        {
            var amountStr = amount.ToString("F2");
            var hash = ComputeHash($"{_merchantId}|{orderRef}|{amountStr}|{_hashKey}");

            var query = $"?merchantId={_merchantId}&orderRefNum={orderRef}" +
                        $"&amount={amountStr}&returnUrl={Uri.EscapeDataString(returnUrl)}" +
                        $"&hash={hash}";

            return _gatewayUrl + query;
        }

        public bool VerifyCallback(IQueryCollection query)
        {
            if (!query.ContainsKey("orderRefNum") || !query.ContainsKey("status") || !query.ContainsKey("hash"))
                return false;

            var expectedHash = ComputeHash($"{query["orderRefNum"]}|{query["status"]}|{_hashKey}");
            return expectedHash == query["hash"];
        }

        private string ComputeHash(string raw)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}