using HRChatbot.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HRChatbot.Services;

public class OpenAIService : IOpenAIService
{
    private readonly IFunctionRegistry _functionRegistry;
    private readonly HttpClient _httpClient;
    private string? _apiKey;

    public OpenAIService(IFunctionRegistry functionRegistry, string? apiKey = null)
    {
        _functionRegistry = functionRegistry;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        SetApiKey(apiKey);
    }

    private void SetApiKey(string? apiKey)
    {
        _apiKey = apiKey;
        if (!string.IsNullOrEmpty(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public void UpdateApiKey(string apiKey)
    {
        SetApiKey(apiKey);
    }

    public async Task<string> SendMessageAsync(string message, List<ChatMessage> history)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not set. Please configure your API key first.");
        }

        try
        {
            var messages = new List<object>();
            
            // Convert history to OpenAI message format
            foreach (var msg in history)
            {
                if (msg.Role == "user" || msg.Role == "assistant")
                {
                    messages.Add(new
                    {
                        role = msg.Role,
                        content = msg.Content
                    });
                }
            }
            
            // Add current user message
            messages.Add(new
            {
                role = "user",
                content = message
            });

            // Get function schemas
            var functionSchemas = _functionRegistry.GetAllFunctionSchemas();
            var tools = functionSchemas.Select(f => new
            {
                type = "function",
                function = new
                {
                    name = f.Name,
                    description = f.Description,
                    parameters = f.Parameters
                }
            }).ToList();

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = messages,
                tools = tools.Count > 0 ? tools : null
            };

            var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions 
            { 
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API error: {response.StatusCode} - {responseContent}");
            }

            var responseJson = JsonNode.Parse(responseContent);
            var choices = responseJson?["choices"]?.AsArray();
            if (choices == null || choices.Count == 0)
            {
                throw new Exception("No response from OpenAI API");
            }

            var firstChoice = choices[0];
            var messageNode = firstChoice?["message"];

            // Handle function calls if present
            var toolCalls = messageNode?["tool_calls"]?.AsArray();
            if (toolCalls != null && toolCalls.Count > 0)
            {
                var toolCall = toolCalls[0];
                var functionName = toolCall?["function"]?["name"]?.ToString();
                var functionArgsJson = toolCall?["function"]?["arguments"]?.ToString();
                var toolCallId = toolCall?["id"]?.ToString();

                if (functionName != null && functionArgsJson != null)
                {
                    var functionArgs = JsonNode.Parse(functionArgsJson);

                    // Execute the function
                    var functionResult = await _functionRegistry.ExecuteFunctionAsync(functionName, functionArgs);
                    var functionResultJson = JsonSerializer.Serialize(functionResult);

                    // Add function call and result to messages
                    messages.Add(new
                    {
                        role = "assistant",
                        content = messageNode?["content"]?.ToString() ?? "",
                        tool_calls = new[]
                        {
                            new
                            {
                                id = toolCallId,
                                type = "function",
                                function = new
                                {
                                    name = functionName,
                                    arguments = functionArgsJson
                                }
                            }
                        }
                    });
                    messages.Add(new
                    {
                        role = "tool",
                        content = functionResultJson,
                        tool_call_id = toolCallId
                    });

                    // Make another API call with the function result
                    var followUpRequestBody = new
                    {
                        model = "gpt-3.5-turbo",
                        messages = messages,
                        tools = tools.Count > 0 ? tools : null
                    };

                    var followUpJson = JsonSerializer.Serialize(followUpRequestBody, new JsonSerializerOptions 
                    { 
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
                    });
                    var followUpContent = new StringContent(followUpJson, Encoding.UTF8, "application/json");

                    var followUpResponse = await _httpClient.PostAsync("chat/completions", followUpContent);
                    var followUpResponseContent = await followUpResponse.Content.ReadAsStringAsync();

                    if (!followUpResponse.IsSuccessStatusCode)
                    {
                        throw new Exception($"OpenAI API error: {followUpResponse.StatusCode} - {followUpResponseContent}");
                    }

                    var followUpResponseJson = JsonNode.Parse(followUpResponseContent);
                    var followUpChoices = followUpResponseJson?["choices"]?.AsArray();
                    if (followUpChoices != null && followUpChoices.Count > 0)
                    {
                        return followUpChoices[0]?["message"]?["content"]?.ToString() ?? "No response generated.";
                    }
                }
            }

            return messageNode?["content"]?.ToString() ?? "No response generated.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        try
        {
            var testClient = new HttpClient();
            testClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            testClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var testRequest = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = "test" }
                },
                max_tokens = 5
            };

            var json = JsonSerializer.Serialize(testRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await testClient.PostAsync("chat/completions", content);
            
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
