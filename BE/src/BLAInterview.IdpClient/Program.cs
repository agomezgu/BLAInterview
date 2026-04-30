using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Duende.IdentityModel.Client;

const string idpBaseUrl = "https://localhost:7007";
const string apiBaseUrl = "https://localhost:7205";
const string userEmail = "candidate@example.com";
const string userPassword = "Str0ngPassword!";

var idpClient = new HttpClient();
var disco = await idpClient.GetDiscoveryDocumentAsync(idpBaseUrl);
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    Console.WriteLine(disco.Exception);
    return 1;
}

var registrationResponse = await idpClient.PostAsJsonAsync(
    $"{idpBaseUrl}/connect/register",
    new
    {
        Name = "Candidate",
        Email = userEmail,
        Password = userPassword
    });

if (!registrationResponse.IsSuccessStatusCode && registrationResponse.StatusCode != HttpStatusCode.Conflict)
{
    Console.WriteLine(registrationResponse.StatusCode);
    return 1;
}

var tokenResponse = await idpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
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

Console.WriteLine("Access token obtained.");

var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken!);
var jsonWriteOptions = new JsonSerializerOptions { WriteIndented = true };

var (errA, taskAId) = await RunTaskAChain(apiClient, jsonWriteOptions);
if (errA is int a)
{
    return a;
}

var (errB, taskBId) = await RunTaskBChain(apiClient, jsonWriteOptions);
if (errB is int b)
{
    return b;
}

var listResponse = await apiClient.GetAsync($"{apiBaseUrl}/tasks");
if (await WriteResponseOrFail(listResponse, "GET /tasks (after both tasks)", jsonWriteOptions) is { } e)
{
    return e;
}

if (await DeleteAndVerifyGone(apiClient, jsonWriteOptions, taskAId, "Task A (completed path)") is { } d0)
{
    return d0;
}

if (await DeleteAndVerifyGone(apiClient, jsonWriteOptions, taskBId, "Task B (cancelled path)") is { } d1)
{
    return d1;
}

var listAfterDelete = await apiClient.GetAsync($"{apiBaseUrl}/tasks");
if (await WriteResponseOrFail(listAfterDelete, "GET /tasks (after deletes)", jsonWriteOptions) is { } e2)
{
    return e2;
}

Console.WriteLine("Done.");
return 0;

async Task<(int? err, int id)> RunTaskAChain(HttpClient http, JsonSerializerOptions options)
{
    const string title = "Status demo - completed path";
    var post = await http.PostAsJsonAsync($"{apiBaseUrl}/tasks", new { title });
    if (await WriteResponseOrFail(post, "Task A: POST /tasks", options) is { } e)
    {
        return (e, 0);
    }

    var taskAId = await ReadCreateTaskIdAsync(post);
    Console.WriteLine($"Task A id: {taskAId}");

    var get0 = await http.GetAsync($"{apiBaseUrl}/tasks/{taskAId}");
    if (await WriteResponseOrFail(get0, "Task A: GET /tasks/{id} (status null or initial)", options) is { } e0)
    {
        return (e0, 0);
    }

    if (await PutStatus(http, options, taskAId, "Pending", "Task A: PUT status Pending") is { } p0)
    {
        return (p0, 0);
    }

    if (await PutStatus(http, options, taskAId, "InProgress", "Task A: PUT status InProgress") is { } p1)
    {
        return (p1, 0);
    }

    if (await PutStatus(http, options, taskAId, "Completed", "Task A: PUT status Completed") is { } p2)
    {
        return (p2, 0);
    }

    // Negative: terminal Completed cannot go to InProgress
    var bad = await http.PutAsJsonAsync($"{apiBaseUrl}/tasks/{taskAId}", new { status = "InProgress" });
    if (bad.IsSuccessStatusCode)
    {
        Console.WriteLine("Task A: expected 400 for invalid status transition, got success.");
        return (1, 0);
    }

    if (bad.StatusCode != HttpStatusCode.BadRequest)
    {
        Console.WriteLine($"Task A: expected BadRequest, got {bad.StatusCode}");
        Console.WriteLine(await bad.Content.ReadAsStringAsync());
        return (1, 0);
    }

    Console.WriteLine("Task A: negative PUT (Completed -> InProgress) returned 400 as expected.");
    var errBody = await bad.Content.ReadAsStringAsync();
    if (!string.IsNullOrWhiteSpace(errBody))
    {
        var doc = JsonDocument.Parse(errBody).RootElement;
        Console.WriteLine(JsonSerializer.Serialize(doc, options));
    }

    return (null, taskAId);
}

async Task<(int? err, int id)> RunTaskBChain(HttpClient http, JsonSerializerOptions options)
{
    const string title = "Status demo - cancelled path";
    var post = await http.PostAsJsonAsync($"{apiBaseUrl}/tasks", new { title });
    if (await WriteResponseOrFail(post, "Task B: POST /tasks", options) is { } e)
    {
        return (e, 0);
    }

    var taskBId = await ReadCreateTaskIdAsync(post);
    Console.WriteLine($"Task B id: {taskBId}");

    var get0 = await http.GetAsync($"{apiBaseUrl}/tasks/{taskBId}");
    if (await WriteResponseOrFail(get0, "Task B: GET /tasks/{id} (status null or initial)", options) is { } e0)
    {
        return (e0, 0);
    }

    if (await PutStatus(http, options, taskBId, "Pending", "Task B: PUT status Pending") is { } p0)
    {
        return (p0, 0);
    }

    if (await PutStatus(http, options, taskBId, "InProgress", "Task B: PUT status InProgress") is { } p1)
    {
        return (p1, 0);
    }

    if (await PutStatus(http, options, taskBId, "Cancelled", "Task B: PUT status Cancelled") is { } p2)
    {
        return (p2, 0);
    }

    return (null, taskBId);
}

static async Task<int?> PutStatus(
    HttpClient http,
    JsonSerializerOptions options,
    int id,
    string status,
    string label)
{
    var response = await http.PutAsJsonAsync($"{apiBaseUrl}/tasks/{id}", new { status });
    return await WriteResponseOrFail(response, label, options);
}

static async Task<int> ReadCreateTaskIdAsync(HttpResponseMessage post)
{
    var json = await post.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(json);
    return doc.RootElement.GetProperty("taskId").GetInt32();
}

static async Task<int?> WriteResponseOrFail(
    HttpResponseMessage response,
    string label,
    JsonSerializerOptions jsonWriteOptions)
{
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"{label} failed: {response.StatusCode}");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        return 1;
    }

    var body = await response.Content.ReadAsStringAsync();
    if (string.IsNullOrWhiteSpace(body))
    {
        Console.WriteLine($"{label} -> {response.StatusCode} (no body)");
        return null;
    }

    var doc = JsonDocument.Parse(body).RootElement;
    Console.WriteLine($"{label}:");
    Console.WriteLine(JsonSerializer.Serialize(doc, jsonWriteOptions));
    return null;
}

async Task<int?> DeleteAndVerifyGone(
    HttpClient http,
    JsonSerializerOptions options,
    int id,
    string label)
{
    var del = await http.DeleteAsync($"{apiBaseUrl}/tasks/{id}");
    if (del.StatusCode != HttpStatusCode.NoContent)
    {
        Console.WriteLine($"{label}: DELETE /tasks/{id} expected 204, got {del.StatusCode}");
        Console.WriteLine(await del.Content.ReadAsStringAsync());
        return 1;
    }

    Console.WriteLine($"{label}: DELETE /tasks/{id} -> 204");
    var get = await http.GetAsync($"{apiBaseUrl}/tasks/{id}");
    if (get.StatusCode != HttpStatusCode.NotFound)
    {
        var errGet = await get.Content.ReadAsStringAsync();
        Console.WriteLine($"{label}: GET after delete expected 404, got {get.StatusCode}");
        if (!string.IsNullOrWhiteSpace(errGet))
        {
            Console.WriteLine(errGet);
        }

        return 1;
    }

    Console.WriteLine($"{label}: GET /tasks/{id} after delete -> 404 as expected.");
    return null;
}
