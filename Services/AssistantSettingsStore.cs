using System.Text.Json;

namespace OfflineCodeAssistant.Services;

public record AssistantSettings(string ServerUrl = "http://127.0.0.1:8080", string ModelName = "local-coder", int MaxTokens = 2048, double Temperature = 0.2);
public record AssistantStatus(AssistantSettings Settings, bool IsConfigured, string? SetupHint);

public sealed class AssistantSettingsStore
{
    private readonly string _path = Path.Combine(AppContext.BaseDirectory, "data", "settings.json");
    private AssistantSettings? _current;

    public AssistantSettings Get() => _current ??= Read();
    public AssistantStatus GetStatus() => new(Get(), true, "Run llama-server from the same USB drive and keep this window open.");
    public async Task SaveAsync(AssistantSettings settings)
    {
        _current = settings;
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        await File.WriteAllTextAsync(_path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
    }
    public string? Validate(AssistantSettings settings) =>
        !Uri.TryCreate(settings.ServerUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttp || !uri.IsLoopback
            ? "Use a local URL such as http://127.0.0.1:8080."
            : settings.MaxTokens is < 64 or > 8192 ? "Max tokens must be between 64 and 8192." : null;
    private AssistantSettings Read()
    {
        try { return JsonSerializer.Deserialize<AssistantSettings>(File.ReadAllText(_path)) ?? new(); }
        catch { return new(); }
    }
}
