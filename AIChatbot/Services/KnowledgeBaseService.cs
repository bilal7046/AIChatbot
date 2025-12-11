using System.Text.Json;
using System.Text.Json.Serialization;

namespace AIChatbot.Services;

public class KnowledgeBaseService
{
    private KnowledgeBase? _knowledgeBase;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<KnowledgeBaseService> _logger;

    public KnowledgeBaseService(IWebHostEnvironment environment, ILogger<KnowledgeBaseService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task LoadKnowledgeBaseAsync()
    {
        try
        {
            var jsonPath = Path.Combine(_environment.WebRootPath, "knowledge-base.json");
            
            if (File.Exists(jsonPath))
            {
                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                _knowledgeBase = JsonSerializer.Deserialize<KnowledgeBase>(jsonContent, options);
                
                if (_knowledgeBase == null)
                {
                    _logger.LogWarning("Knowledge base deserialized as null, using default");
                    _knowledgeBase = GetDefaultKnowledgeBase();
                }
                else
                {
                    _logger.LogInformation("Knowledge base loaded successfully. Navigation items: {Count}, Services items: {ServicesCount}, Status items: {StatusCount}", 
                        _knowledgeBase.Navigation?.Count ?? 0,
                        _knowledgeBase.Services?.Count ?? 0,
                        _knowledgeBase.Status?.Count ?? 0);
                }
            }
            else
            {
                _logger.LogWarning("Knowledge base file not found at {Path}, using default", jsonPath);
                _knowledgeBase = GetDefaultKnowledgeBase();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading knowledge base");
            _knowledgeBase = GetDefaultKnowledgeBase();
        }
    }

    public string ProcessMessage(string message)
    {
        if (_knowledgeBase == null)
        {
            _logger.LogWarning("Knowledge base is null, using default");
            _knowledgeBase = GetDefaultKnowledgeBase();
        }

        var lowerMessage = message.ToLowerInvariant();
        string? response = null;

        _logger.LogDebug("Processing message: {Message}", message);

        // Check navigation
        if (_knowledgeBase.Navigation != null && _knowledgeBase.Navigation.Any())
        {
            foreach (var item in _knowledgeBase.Navigation)
            {
                if (item.Keywords != null && item.Keywords.Any(keyword => lowerMessage.Contains(keyword.ToLowerInvariant())))
                {
                    if (item.Responses != null && item.Responses.Any())
                    {
                        var random = new Random();
                        response = item.Responses[random.Next(item.Responses.Count)];
                        _logger.LogDebug("Matched navigation keyword, response selected");
                        break;
                    }
                }
            }
        }

        // Check services
        if (string.IsNullOrEmpty(response) && _knowledgeBase.Services != null && _knowledgeBase.Services.Any())
        {
            foreach (var item in _knowledgeBase.Services)
            {
                if (item.Keywords != null && item.Keywords.Any(keyword => lowerMessage.Contains(keyword.ToLowerInvariant())))
                {
                    if (item.Responses != null && item.Responses.Any())
                    {
                        var random = new Random();
                        response = item.Responses[random.Next(item.Responses.Count)];
                        _logger.LogDebug("Matched services keyword, response selected");
                        break;
                    }
                }
            }
        }

        // Check status
        if (string.IsNullOrEmpty(response) && _knowledgeBase.Status != null && _knowledgeBase.Status.Any())
        {
            foreach (var item in _knowledgeBase.Status)
            {
                if (item.Keywords != null && item.Keywords.Any(keyword => lowerMessage.Contains(keyword.ToLowerInvariant())))
                {
                    if (item.Responses != null && item.Responses.Any())
                    {
                        var random = new Random();
                        response = item.Responses[random.Next(item.Responses.Count)];
                        _logger.LogDebug("Matched status keyword, response selected");
                        break;
                    }
                }
            }
        }

        // Default response
        if (string.IsNullOrEmpty(response))
        {
            _logger.LogWarning("No keyword match found for message: {Message}", message);
            response = $"I understand you're asking about: \"{message}\". Could you please rephrase your question? I can help with navigation, services, or status inquiries.";
        }

        return response;
    }

    private KnowledgeBase GetDefaultKnowledgeBase()
    {
        return new KnowledgeBase
        {
            Navigation = new List<KnowledgeItem>
            {
                new KnowledgeItem
                {
                    Keywords = new List<string> { "where", "location", "find", "directions", "how to get", "navigate", "address", "office", "visit" },
                    Responses = new List<string>
                    {
                        "You can find us at 123 Main Street, Downtown District. We're open Monday to Friday, 9 AM to 5 PM, and Saturday 10 AM to 2 PM.",
                        "Our office is located in the downtown area at 123 Main Street. You can reach us by taking the metro to Central Station, then it's a 5-minute walk.",
                        "For directions to our office, please visit our website or call us at +1-234-567-8900. We're located at 123 Main Street."
                    }
                }
            },
            Services = new List<KnowledgeItem>
            {
                new KnowledgeItem
                {
                    Keywords = new List<string> { "service", "what do you", "offer", "provide", "help with", "can you", "what services", "capabilities", "do you do" },
                    Responses = new List<string>
                    {
                        "We offer a wide range of services including business consulting, technology solutions, software development, and customer support. How can I assist you today?",
                        "Our services include business consulting, technology solutions, digital transformation, and ongoing support. What specific area interests you?",
                        "We provide comprehensive solutions for your business needs including strategy consulting, custom software development, and IT support. Would you like to know more about a specific service?"
                    }
                }
            },
            Status = new List<KnowledgeItem>
            {
                new KnowledgeItem
                {
                    Keywords = new List<string> { "status", "check", "order", "request", "track", "update", "when", "progress", "where is" },
                    Responses = new List<string>
                    {
                        "Your request is currently being processed. Expected completion: within 2-3 business days. You'll receive an email notification when it's ready.",
                        "I've checked your status - everything is on track. Your request is in progress and should be completed within the next 2-3 business days.",
                        "Status: In Progress. Our team is working on your request and will notify you via email upon completion. Expected timeline: 2-3 business days."
                    }
                }
            }
        };
    }
}

public class KnowledgeBase
{
    [JsonPropertyName("navigation")]
    public List<KnowledgeItem> Navigation { get; set; } = new();
    
    [JsonPropertyName("services")]
    public List<KnowledgeItem> Services { get; set; } = new();
    
    [JsonPropertyName("status")]
    public List<KnowledgeItem> Status { get; set; } = new();
}

public class KnowledgeItem
{
    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; } = new();
    
    [JsonPropertyName("responses")]
    public List<string> Responses { get; set; } = new();
}

