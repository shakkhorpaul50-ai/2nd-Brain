using OfflineCodeAssistant.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<AssistantSettingsStore>();
builder.Services.AddHttpClient<LlamaClient>(client => client.Timeout = TimeSpan.FromMinutes(10));

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();
app.MapGet("/api/status", (AssistantSettingsStore settings) => Results.Ok(settings.GetStatus()));
app.MapPost("/api/settings", async (AssistantSettings input, AssistantSettingsStore settings) =>
{
    var error = settings.Validate(input);
    if (error is not null) return Results.BadRequest(new { error });
    await settings.SaveAsync(input);
    return Results.Ok(settings.GetStatus());
});
app.MapPost("/api/chat", async (ChatRequest request, LlamaClient llama, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Message)) return Results.BadRequest(new { error = "Enter a message first." });
    try { return Results.Ok(await llama.CompleteAsync(request, cancellationToken)); }
    catch (HttpRequestException) { return Results.Problem("The local model server is not running. Start it from the Setup panel, then try again.", statusCode: 503); }
    catch (TaskCanceledException) { return Results.Problem("The local model took too long to respond.", statusCode: 504); }
});
app.Run();
