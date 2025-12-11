using System.Net.Http.Json;
using System.Text.Json;

namespace AIChatbot.Services;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAIService> _logger;
    private readonly string? _apiKey;
    private readonly bool _isConfigured;

    public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _apiKey = "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";
        
        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _isConfigured = true;
            _logger.LogInformation("OpenAI service initialized successfully");
        }
        else
        {
            _logger.LogWarning("OpenAI API key not configured. OpenAI features will be disabled.");
            _isConfigured = false;
        }
    }

    public bool IsAvailable => _isConfigured && !string.IsNullOrWhiteSpace(_apiKey);

    public async Task<string> GetChatResponseAsync(string userMessage, List<ChatMessage>? conversationHistory = null)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException("OpenAI service is not configured or available");
        }

        try
        {
            var messages = new List<object>();

            // Add system message with context
            messages.Add(new
            {
                role = "system",
                content = "You are a helpful AI assistant for JMM Innovations. " +
                         "You help customers with navigation guidance, service explanations, and status inquiries. " +
                         "Be friendly, concise, and professional. " +
                         "If asked about location, mention: 123 Main Street, Downtown District. " +
                         "If asked about services, mention: business consulting, technology solutions, software development, and customer support. " +
                         "If asked about status, provide helpful information about request processing times (typically 2-3 business days)."
            });

            // Add conversation history if provided
            if (conversationHistory != null && conversationHistory.Any())
            {
                foreach (var msg in conversationHistory.TakeLast(10)) // Limit to last 10 messages for context
                {
                    messages.Add(new
                    {
                        role = msg.IsUser ? "user" : "assistant",
                        content = msg.Text
                    });
                }
            }

            // Add current user message
            messages.Add(new
            {
                role = "user",
                content = userMessage
            });

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = messages,
                temperature = 0.7,
                max_tokens = 500
            };

            var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                var choices = jsonResponse.GetProperty("choices");
                if (choices.GetArrayLength() > 0)
                {
                    var message = choices[0].GetProperty("message").GetProperty("content").GetString();
                    return message ?? "I apologize, but I couldn't generate a response. Please try again.";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("OpenAI API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                throw new Exception($"OpenAI API error: {response.StatusCode}");
            }

            return "I apologize, but I couldn't generate a response. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            throw;
        }
    }
}

public class ChatMessage
{
    public bool IsUser { get; set; }
    public string Text { get; set; } = string.Empty;
}
