using System.Net.Http.Json;
using System.Text.Json;
using Duende.IdentityModel.Client;

const string userEmail = "candidate@example.com";
const string userPassword = "Str0ngPassword!";

// discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7007");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    Console.WriteLine(disco.Exception);
    return 1;
}

// seed the simulated user when the in-memory IdP has just started
var registrationResponse = await client.PostAsJsonAsync(
    "https://localhost:7007/connect/register",
    new
    {
        Name = "Candidate",
        Email = userEmail,
        Password = userPassword
    });

if (!registrationResponse.IsSuccessStatusCode && registrationResponse.StatusCode != System.Net.HttpStatusCode.Conflict)
{
    Console.WriteLine(registrationResponse.StatusCode);
    return 1;
}

// request token for the registered user
var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "bla-interview-api-client",
    ClientSecret = "secret",
    Scope = "bla-interview-api",
    UserName = userEmail,
    Password = userPassword
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    Console.WriteLine(tokenResponse.ErrorDescription);
    return 1;
}

Console.WriteLine(tokenResponse.AccessToken);

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken!); // AccessToken is always non-null when IsError is false

var response = await apiClient.GetAsync("https://localhost:7205/tasks");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
    return 1;
}

var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
Console.ReadLine();
return 0;