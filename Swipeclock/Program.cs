using System.Net.Http.Headers;
using Swipeclock;

// SEE: https://twpapi.payrollservers.us/swagger/ui/index

using var client = new HttpClient();

// TODO
int siteId = 105495; // Customer's Swipeclock site ID

// Step 1: Generate signed JWT
string jwt = JwtSignature.CreateJwt(siteId);

// Step 2: Exchange JWT for authentication token
string token = await JwtSignature.GetUserTokenAsync(client, jwt);

// Step 3a - get employees
string url = $"https://twpapi.payrollservers.us/company/organizationLevels";
using var request = new HttpRequestMessage(HttpMethod.Get, url);
request.Headers.Authorization =
    new AuthenticationHeaderValue("Bearer", token);
using var response = await client.SendAsync(request);
string result = await response.Content.ReadAsStringAsync();
response.EnsureSuccessStatusCode();
Console.WriteLine(result);

// Step 3 - get timecards
// TODO:

// Step 3 - gets employee logins: Call TimeWorksPlus API
// string url = $"https://twpapi.payrollservers.us/api/{siteId}/logins";
// using var request = new HttpRequestMessage(HttpMethod.Get, url);
// request.Headers.Authorization =
//     new AuthenticationHeaderValue("Bearer", token);
// using var response = await client.SendAsync(request);
// string result = await response.Content.ReadAsStringAsync();
// response.EnsureSuccessStatusCode();
// Console.WriteLine(result);

// TODO: push data to Big Query
