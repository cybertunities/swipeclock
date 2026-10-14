
// Part 1 - authenticate Swipeclock
// SEE: https://developer.swipeclock.com/documentation/authentication/authentication-using-a-jwt-token/

// TODO: Part 1a - generate JWT token

// TODO: Part 1b - use JWT token to get an API token
// POST https://clock.payrollservers.us/AuthenticationService/oauth2/userToken
// May need to use Authorize Bearer before token
// Headers: Accept application/json
// Content-Type application/json

// TODO: pull Timesheet Data
// SEE: https://api.workforcehub.com/swagger/index.html#overview--authentication

// TODO: push data to Big Query

using System.Text.Json;
using Swipeclock;

public class Program
{
    public static void Main(string[] args)
    {
        string jwt = JwtSignature.CreateJwt();
        Console.WriteLine($"Generated JWT Token: {jwt}");
    }
}