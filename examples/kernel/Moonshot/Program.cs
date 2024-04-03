using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

var app = builder.Build();

// Get your api token and default model.
var configJson = await File.ReadAllTextAsync("config.json");
var config = JsonSerializer.Deserialize(configJson, AppJsonContext.Default.CustomConfig);

var apiKey = config?.Token ?? throw new ArgumentNullException(nameof(config.Token));
var defaultModel = config?.DefaultModel ?? throw new ArgumentNullException(nameof(config.DefaultModel));

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
httpClient.BaseAddress = new Uri("https://api.moonshot.cn/v1/"); // Replace with your API base URL.

var todosApi = app.MapGroup("/v1");
todosApi.MapGet("/models", () => GetModelsAsync());
todosApi.MapPost("/chat/completions", GetChatCompletionWithStreamAsync);

// Must print success message to stdout for the server to be considered started.
Console.WriteLine(JsonSerializer.Serialize(new LaunchInfo { Status = LaunchStatus.Success, Message = "Server started successfully." }, AppJsonContext.Default.LaunchInfo));

app.Run("http://localhost:5259");


IResult GetModelsAsync()
{
    var content = httpClient.GetStringAsync("models").Result;
    return Results.Text(content, "application/json", Encoding.UTF8, 200);
}

async Task GetChatCompletionWithStreamAsync(HttpContext context)
{
    var request = context.Request.ReadFromJsonAsync<BasicChatCompletionRequest>().Result;
    request!.Model = defaultModel;

    var req = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
    {
        Content = new StringContent(JsonSerializer.Serialize(request, AppJsonContext.Default.BasicChatCompletionRequest), Encoding.UTF8, "application/json")
    };

    // We don't need deserialization here, so we just read the stream and write it to the response.
    if (request.Stream)
    {
        context.Response.ContentType = "text/event-stream";
        var response = await httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            Console.WriteLine(line);
            await context.Response.WriteAsync(line!.Trim(), Encoding.UTF8);
            await context.Response.WriteAsync("\n", Encoding.UTF8);
            await context.Response.Body.FlushAsync();
        }
    }
    else
    {
        context.Response.ContentType = "application/json";
        var response = await httpClient.SendAsync(req);
        var completion = await response.Content.ReadAsStringAsync();
        await context.Response.WriteAsync(completion);
        await context.Response.Body.FlushAsync();
    }
}

[JsonSerializable(typeof(CustomConfig))]
[JsonSerializable(typeof(LaunchInfo))]
[JsonSerializable(typeof(BasicChatMessage))]
[JsonSerializable(typeof(BasicChatCompletionRequest))]
internal partial class AppJsonContext : JsonSerializerContext { }

internal class CustomConfig
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("default_model")]
    public string? DefaultModel { get; set; }
}

enum LaunchStatus
{
    Success = 0,
    Error = 1,
}

internal class LaunchInfo
{
    [JsonPropertyName("status")]
    public LaunchStatus Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

internal class BasicChatMessage
{
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

internal class BasicChatCompletionRequest
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("messages")]
    public List<BasicChatMessage>? Messages { get; set; }

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public float TopP { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("repetition_penalty")]
    public float RepetitionPenalty { get; set; }
}
