using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class SwipeclockHelper
{
    private static readonly HttpClient client = new();

    public static async Task<string> HttpGetAsync(string endpoint, Dictionary<string, string> queryParams, string token)
    {
        const string API_VERSION = "1";
        const string PARTNER_ID = "2385";

        string queryString = string.Join("&",
            queryParams.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

        string url = $"{endpoint}?{queryString}";

        // Create request
        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

        // Add headers
        // request.Headers.Authorization =
        //     new AuthenticationHeaderValue("Bearer", token);

        // request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
        client.DefaultRequestHeaders.Add("x-api-version", API_VERSION);
        client.DefaultRequestHeaders.Add("x-integration-partner-id", PARTNER_ID);
        // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Send request
        using HttpResponseMessage response = await client.SendAsync(request);

        // Throw exception if HTTP status is not successful
        response.EnsureSuccessStatusCode();

        // Read response
        string result = await response.Content.ReadAsStringAsync();

        return result;
    }
}