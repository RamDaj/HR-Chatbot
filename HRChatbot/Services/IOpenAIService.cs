using HRChatbot.Models;

namespace HRChatbot.Services;

public interface IOpenAIService
{
    Task<string> SendMessageAsync(string message, List<ChatMessage> history);
    Task<bool> ValidateApiKeyAsync(string apiKey);
    void UpdateApiKey(string apiKey);
}
