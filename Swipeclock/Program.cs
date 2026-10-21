using Swipeclock;

// Part 1 - authenticate Swipeclock
// SEE: https://developer.swipeclock.com/documentation/authentication/authentication-using-a-jwt-token/

// Part 1a - generate JWT token
string jwt = JwtSignature.CreateJwt();
Console.WriteLine($"Generated JWT Token: {jwt}");

// TODO: Part 1b - use JWT token to get an API token
// POST https://clock.payrollservers.us/AuthenticationService/oauth2/userToken
// May need to use Authorize Bearer before token
// Headers: Accept application/json
// Content-Type application/json

string? apiToken = await JwtSignature.GetUserTokenAsync(jwt);

if (apiToken == null)
{
    Console.WriteLine("Failed to retrieve API token.");
    return;
}

Console.WriteLine($"Generated API Token: {apiToken}");

// TODO: pull Timesheet Data
// SEE: https://api.workforcehub.com/swagger/index.html#overview--authentication

var endpoint = "https://api.workforcehub.com/api/company";

var queryString = new Dictionary<string, string>()
{
    { "idType", "CompanyCode" }
};

await SwipeclockHelper.HttpGetAsync(endpoint, queryString, apiToken)
    .ContinueWith(task =>
    {
        if (task.IsCompletedSuccessfully)
        {
            string responseBody = task.Result;
            Console.WriteLine($"Timesheet Data: {responseBody}");
        }
        else
        {
            Console.WriteLine($"Error retrieving timesheet data: {task.Exception?.Message}");
        }
    });

// TODO: push data to Big Query
