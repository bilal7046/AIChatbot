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
            var systemMessage = "You are a friendly customer service representative helping users with Absher (the Saudi Arabian government portal). " +
                         "Write like a real person - use natural, conversational language. Avoid sounding robotic or overly formal. " +
                         "Use contractions (I'm, you're, it's), casual phrases, and friendly tone. " +
                         "Use simple words and avoid technical jargon. Write like you're texting a friend, but still be professional. " +
                         "IMPORTANT: Never use any icons, emojis, symbols, charts, graphs, or visual elements in your responses. Use only plain text. " +
                         "Do not use any Unicode symbols, special characters, or visual indicators. Just write normal text. " +
                         "Focus on Absher services: Civil Affairs (الأحوال المدنية), Traffic Services (المرور), Labor Services (العمل), and other government services. " +
                         "Never mention JMM Innovations or any other company. You're an Absher support person. ";

            // Check if this is a Service Explanation request
            if (userMessage.Contains("[Category: Service Explanation]", StringComparison.OrdinalIgnoreCase))
            {
                systemMessage += "SPECIFIC INSTRUCTIONS FOR SERVICE EXPLANATION: " +
                               "When explaining Absher services, provide clear, simple explanations of how specific services work and what steps are required. " +
                               "Break down the process into easy-to-follow steps. Explain what documents are needed, what the user needs to do, and what happens next. " +
                               "Write in flowing sentences, but make sure each step is clear. Don't use numbered lists or bullet points - just explain naturally. " +
                               "Keep it practical and actionable. Tell them exactly what they need to do, where to go, and what to expect. " +
                               "If they ask about a specific service (like ID renewal, passport, work permit, driving license), explain the complete process from start to finish. " +
                               "Be thorough but keep it simple and easy to understand.";
            }
            else if (userMessage.Contains("[Category: Navigation Guidance]", StringComparison.OrdinalIgnoreCase))
            {
                systemMessage += "SPECIFIC INSTRUCTIONS FOR NAVIGATION GUIDANCE: " +
                               "For navigation questions, provide clear step-by-step guidance on finding things in Absher. " +
                               "Use bullet points (use - for bullets) to list the steps clearly. " +
                               "Break down the navigation process into bulleted steps showing exactly where to go on the website. " +
                               "Tell them where to look, what menu to use, and how to get to the service they need. " +
                               "Format: Start with a brief explanation, then list the steps using bullets with - prefix. " +
                               "Example format: 'To find [service], follow these steps:\n- Step 1\n- Step 2\n- Step 3'";
            }
            else if (userMessage.Contains("[Category: Status Inquiries]", StringComparison.OrdinalIgnoreCase))
            {
                systemMessage += "For status questions, the system automatically checks ID numbers. Just give the status naturally without over-explaining.";
            }
            else
            {
                systemMessage += "Keep responses short and to the point. Don't use bullet points unless absolutely necessary - write in flowing sentences. " +
                               "Don't say things like 'I understand', 'I can help you', 'Let me explain' - just get straight to the answer.";
            }

            messages.Add(new
            {
                role = "system",
                content = systemMessage
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
