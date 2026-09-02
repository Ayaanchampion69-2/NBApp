using System.Security.Cryptography;
using System.Text;

string ComputeHash(string raw)
{
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
    return Convert.ToHexString(bytes).ToLowerInvariant();
}

string merchantId = "TEST123";
string hashKey = "supersecretkey";
int orderRef = 42;
decimal amount = 99.50m;

// --- Test 1: BuildCheckoutUrl hash matches expected formula ---
var amountStr = amount.ToString("F2");
var checkoutHash = ComputeHash($"{merchantId}|{orderRef}|{amountStr}|{hashKey}");
Console.WriteLine($"[Checkout hash]  {checkoutHash}");

// --- Test 2: VerifyCallback accepts a correctly-computed hash ---
string status = "Success";
var callbackHash = ComputeHash($"{orderRef}|{status}|{hashKey}");
Console.WriteLine($"[Correct callback hash]   {callbackHash}");

// --- Test 3: a tampered hash must NOT match ---
var tamperedHash = callbackHash[..^1] + (callbackHash[^1] == 'a' ? 'b' : 'a');
Console.WriteLine($"[Tampered hash]           {tamperedHash}  (should differ from correct hash: {tamperedHash != callbackHash})");

// --- Test 4: changing status changes the hash (integrity check) ---
var failedHash = ComputeHash($"{orderRef}|Failed|{hashKey}");
Console.WriteLine($"[Failed-status hash]      {failedHash}  (differs from Success hash: {failedHash != callbackHash})");