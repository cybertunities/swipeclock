using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;

namespace Swipeclock;

using System.Text.Json;

internal class SwipeclockJwtHeader
{
    public string alg { get; set; }
    public string typ { get; set; }
}

/**
class SwipeclockSiteInfo
{
    internal string type { get; set; }
    internal string id { get; set; }
}
*/
internal class SwipeclockJwtPayload
{
    public int iss { get; set; }
    public string product { get; set; }
    public string sub { get; set; }

    public long exp { get; set; }

    public object siteInfo { get; set; }
    // internal SwipeclockSiteInfo siteInfo { get; set; }
}

internal class JwtHelper
{
    internal static string CreateJwt(string headerJson, string payloadJson, string apiSecret)
    {
        // 1. Base64URL encode header and payload
        string header = Base64UrlEncode(
            Encoding.UTF8.GetBytes(headerJson));

        string payload = Base64UrlEncode(
            Encoding.UTF8.GetBytes(payloadJson));

        // 2. Combine header and payload
        string signingInput = $"{header}.{payload}";

        // 3. Sign using API secret and HMAC-SHA256
        byte[] key = Encoding.ASCII.GetBytes(apiSecret);
        byte[] data = Encoding.ASCII.GetBytes(signingInput);

        byte[] signature = HMACSHA256.HashData(key, data);

        // 4. Base64URL encode signature
        string encodedSignature = Base64UrlEncode(signature);

        // 5. Return complete JWT
        return $"{signingInput}.{encodedSignature}";
    }

    private static string Base64UrlEncode(byte[] data)
    {
        return Convert.ToBase64String(data)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

public class JwtSignature
{
    const string SWIPECLOCK_SECRET = "qnWrACmWiE1MBOrB92NlNwAaafMru3a7pC2s25kSPNnbI6aQlELM1ad9vrIIaHoI";

    public static string CreateJwt(int siteId)
    {
        var header = new
        {
            alg = "HS256",
            typ = "JWT"
        };

        var payload = new
        {
            iss = "2385",
            sub = "partner",
            exp = DateTimeOffset.UtcNow
                .AddMinutes(3)
                .ToUnixTimeSeconds(),
            product = "twppartner",
            siteInfo = new
            {
                type = "id",
                id = siteId
            }
        };

        string headerJson = JsonSerializer.Serialize(header);
        string payloadJson = JsonSerializer.Serialize(payload);

        return JwtHelper.CreateJwt(
            headerJson,
            payloadJson,
            SWIPECLOCK_SECRET
        );
    }


    public static async Task<string> GetUserTokenAsync(
        HttpClient client,
        string jwtToken)
    {
        const string url =
            "https://clock.payrollservers.us/AuthenticationService/oauth2/usertoken";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", jwtToken);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await client.SendAsync(request);

        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Swipeclock authentication failed ({(int)response.StatusCode}): {responseBody}");
        }

        using var json = JsonDocument.Parse(responseBody);

        if (!json.RootElement.TryGetProperty("token", out var tokenElement))
        {
            throw new InvalidOperationException(
                "Swipeclock authentication response did not contain a token.");
        }

        string? token = tokenElement.GetString();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Swipeclock returned an empty authentication token.");
        }

        return token;
    }
}