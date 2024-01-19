using IdentityModel.Client;
using Spectre.Console;
using Spectre.Console.Json;
using System.Text;
using System.Text.Json;
using TestApp;

Console.WriteLine("Press any key to start..");
Console.ReadKey();

Console.WriteLine();
Console.WriteLine("Getting access token..");

string accessToken = string.Empty;
try
{
    var accessTokenClient = new HttpClient();
    var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_id", "client" },
                { "client_secret", "secret" },
                { "scope", "post.read post.create comment.create" },
                { "grant_type", "client_credentials" },
            };
    var requestContent = new FormUrlEncodedContent(tokenRequestParameters);

    using HttpResponseMessage response = await accessTokenClient.PostAsync("http://localhost:8002/connect/token", requestContent).ConfigureAwait(false);
    var token = await TokenResponse.FromHttpResponseAsync<TokenResponse>(response);
    if (!token.IsError)
    {
        accessToken = token.AccessToken;
        AnsiConsole.MarkupLine($"[green]Got Access Token:[/] {token.AccessToken}");
    }
    else
    {
        AnsiConsole.MarkupLine($"Access Token error: [red]{token.Error}[/]");
        Exit();
    }
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"Access Token request error: [red]{ex.Message}[/]");
    Exit();
}

var apiClient = new HttpClient()
{
    BaseAddress = new Uri("http://localhost:8001")
};
apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

await Task.Delay(1000);
Console.WriteLine();
Console.WriteLine("Creating new Post..");

long postId = 0;
try
{
    var postResponse = await apiClient.PostAsync("/api/Posts",
        new StringContent(JsonSerializer.Serialize(new CreatePostRequest
        {
            CreatedBy = "ArmaganE",
            Title = "Test Post",
            Content = "Test Post Content"
        }), Encoding.UTF8, "application/json"));

    string postResponseColor = postResponse.IsSuccessStatusCode ? "green" : "red";
    AnsiConsole.MarkupLine($"HttpStatusCode: [{postResponseColor}]{postResponse.StatusCode}[/]");
    var postResponseString = await postResponse.Content.ReadAsStringAsync();

    if(!postResponse.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine($"Post create response: [{postResponseColor}]{(postResponseString)}[/]");
        Exit();
    }

    var createPostResponse = JsonSerializer.Deserialize<CreatePostResponse>(postResponseString, jsonSerializerOptions);
    postId = createPostResponse.Id;

    AnsiConsole.Write(new JsonText(postResponseString));
    Console.WriteLine();

    AnsiConsole.MarkupLine($"PostId: [{postResponseColor}]{(postId)}[/]");
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"Post create request error: [red]{ex.Message}[/]");
    Exit();
}

await Task.Delay(1000);
Console.WriteLine();
Console.WriteLine($"Adding comments to post {postId}.");

try
{
    Console.WriteLine();
    Console.WriteLine("Adding first comment.");

    var commentResponse1 = await apiClient.PostAsync("/api/Posts/Comments",
        new StringContent(JsonSerializer.Serialize(new CreateCommentRequest
        {
            PostId = postId,
            CreatedBy = "ArmaganE",
            Content = "Test Comment 1"
        }), Encoding.UTF8, "application/json"));

    string commentResponseColor1 = commentResponse1.IsSuccessStatusCode ? "green" : "red";
    AnsiConsole.MarkupLine($"HttpStatusCode: [{commentResponseColor1}]{commentResponse1.StatusCode}[/]");
    var commentResponseString1 = await commentResponse1.Content.ReadAsStringAsync();

    if (!commentResponse1.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine($"Comment create response: [{commentResponseColor1}]{(commentResponseString1)}[/]");
        Exit();
    }

    var comment1 = JsonSerializer.Deserialize<CreateCommentResponse>(commentResponseString1, jsonSerializerOptions);
    AnsiConsole.Write(new JsonText(commentResponseString1));
    Console.WriteLine();
    await Task.Delay(1000);
    Console.WriteLine();
    Console.WriteLine("Adding second comment.");

    var commentResponse2 = await apiClient.PostAsync("/api/Posts/Comments",
    new StringContent(JsonSerializer.Serialize(new CreateCommentRequest
    {
        PostId = postId,
        CreatedBy = "ArmaganE",
        Content = "Test Comment 2"
    }), Encoding.UTF8, "application/json"));

    string commentResponseColor2 = commentResponse2.IsSuccessStatusCode ? "green" : "red";
    AnsiConsole.MarkupLine($"HttpStatusCode: [{commentResponseColor2}]{commentResponse2.StatusCode}[/]");
    var commentResponseString2 = await commentResponse2.Content.ReadAsStringAsync();


    if (!commentResponse2.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine($"Post create response: [{commentResponseColor2}]{(commentResponseString2)}[/]");
        Exit();
    }

    var comment2 = JsonSerializer.Deserialize<CreateCommentResponse>(commentResponseString2, jsonSerializerOptions);
    AnsiConsole.Write(new JsonText(commentResponseString2));
    Console.WriteLine();
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"Comment create request error: [red]{ex.Message}[/]");
    Exit();
}

await Task.Delay(1000);
Console.WriteLine();
Console.WriteLine($"Getting Post {postId}.");

try
{
    var postResponse = await apiClient.GetAsync($"/api/Posts/{postId}");
    string postResponseColor = postResponse.IsSuccessStatusCode ? "green" : "red";
    AnsiConsole.MarkupLine($"HttpStatusCode: [{postResponseColor}]{postResponse.StatusCode}[/]");
    var postResponseString = await postResponse.Content.ReadAsStringAsync();

    if (!postResponse.IsSuccessStatusCode)
    {
        AnsiConsole.MarkupLine($"Post get response: [{postResponseColor}]{(postResponseString)}[/]");
        Exit();
    }

    AnsiConsole.Write(new JsonText(postResponseString));
    Console.WriteLine();
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"Post get request error: [red]{ex.Message}[/]");
    Exit();
}

CancellationTokenSource cts = new();
ParallelOptions options = new() { CancellationToken = cts.Token };
int max = 20;

await Task.Delay(1000);
Console.WriteLine();
Console.WriteLine($"Testing rate limiter with {max} requests in parallel (API limit: max 5 requests in 5 seconds.)");

try
{
    await Parallel.ForAsync(1, max, options, async (i, ct) =>
    {
        if (cts.IsCancellationRequested) return;

        Console.WriteLine($"Requesting post. ({i} / {max})");

        var postResponse = await apiClient.GetAsync($"/api/Posts/{postId}", cts.Token);

        AnsiConsole.MarkupLine($"HttpStatusCode: [{(postResponse.IsSuccessStatusCode ? "green" : "red")}]{postResponse.StatusCode}[/] ({i} / {max}).");

        if (!postResponse.IsSuccessStatusCode)
            cts.Cancel();
    });
}
catch (OperationCanceledException cex)
{
    Console.WriteLine("Stopping rate limiter test.");
}

Console.WriteLine();
Exit();

static void Exit()
{
    Console.WriteLine("Press any key to quit..");
    Console.ReadKey();
    Environment.Exit(0);
}
