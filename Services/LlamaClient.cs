using System.Net.Http.Json;
using System.Text.Json;

namespace OfflineCodeAssistant.Services;

public sealed class LlamaClient(HttpClient http, AssistantSettingsStore settings)
{
    private const string SystemPrompt = "You are Offline Code Assistant. Help the user build and debug C#, ASP.NET Razor (.cshtml), and CSS. Be precise, give complete code when asked, explain steps briefly, and never claim to access the internet. Format code in fenced blocks with the language name.";

    public async Task<ChatResponse> CompleteAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var options = settings.Get();
        var messages = new List<ChatMessage> { new("system", SystemPrompt) };
        if (request.History is not null) messages.AddRange(request.History.TakeLast(12).Where(x => x.Role is "user" or "assistant"));
        messages.Add(new("user", request.Message));
        var endpoint = options.ServerUrl.TrimEnd('/') + "/v1/chat/completions";
        using var response = await http.PostAsJsonAsync(endpoint, new { model = options.ModelName, messages, temperature = options.Temperature, max_tokens = options.MaxTokens, stream = false }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        var content = payload.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        return new ChatResponse(content ?? "The local model returned an empty response.");
    }
}
