namespace OfflineCodeAssistant.Services;

public record ChatRequest(string Message, List<ChatMessage>? History);
public record ChatMessage(string Role, string Content);
public record ChatResponse(string Content);
