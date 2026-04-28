
using System.Text.Json;
using Duende.IdentityModel.Client;

// discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7007");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    Console.WriteLine(disco.Exception);
    return 1;
}

// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,
    ClientId = "bla-interview-api-client",
    ClientSecret = "secret",
    Scope = "bla-interview-api"
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