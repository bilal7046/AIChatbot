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
            // Try widget directory first (at project root level)
            var widgetPath = Path.Combine(_environment.ContentRootPath, "..", "widget", "knowledge-base.json");
            widgetPath = Path.GetFullPath(widgetPath);
            
            // Fallback to wwwroot if widget path doesn't exist
            var jsonPath = File.Exists(widgetPath) ? widgetPath : Path.Combine(_environment.WebRootPath, "knowledge-base.json");
            var sourceLocation = File.Exists(widgetPath) ? "widget/knowledge-base.json" : "wwwroot/knowledge-base.json";
            
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
                    _logger.LogInformation("Knowledge base loaded successfully from {Source}. Navigation items: {Count}, Services items: {ServicesCount}, Status items: {StatusCount}", 
                        sourceLocation,
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

    public string ProcessMessage(string message, string? category = null)
    {
        if (_knowledgeBase == null)
        {
            _logger.LogWarning("Knowledge base is null, using default");
            _knowledgeBase = GetDefaultKnowledgeBase();
        }

        var lowerMessage = message.ToLowerInvariant();
        string? response = null;

        _logger.LogDebug("Processing message: {Message} with category: {Category}", message, category);

        // When category is specified, prioritize that category's responses
        if (category == "Service Explanation")
        {
            // Check services FIRST for Service Explanation category
            if (_knowledgeBase.Services != null && _knowledgeBase.Services.Any())
            {
                foreach (var item in _knowledgeBase.Services)
                {
                    if (item.Keywords != null && item.Keywords.Any(keyword => lowerMessage.Contains(keyword.ToLowerInvariant())))
                    {
                        if (item.Responses != null && item.Responses.Any())
                        {
                            var random = new Random();
                            response = item.Responses[random.Next(item.Responses.Count)];
                            _logger.LogDebug("Matched services keyword for Service Explanation, response selected");
                            break;
                        }
                    }
                }
            }
            
            // Only check navigation if no service match found
            if (string.IsNullOrEmpty(response) && _knowledgeBase.Navigation != null && _knowledgeBase.Navigation.Any())
            {
                foreach (var item in _knowledgeBase.Navigation)
                {
                    if (item.Keywords != null && item.Keywords.Any(keyword => lowerMessage.Contains(keyword.ToLowerInvariant())))
                    {
                        if (item.Responses != null && item.Responses.Any())
                        {
                            var random = new Random();
                            response = item.Responses[random.Next(item.Responses.Count)];
                            _logger.LogDebug("Matched navigation keyword as fallback, response selected");
                            break;
                        }
                    }
                }
            }
        }
        else if (category == "Navigation Guidance")
        {
            // Check navigation FIRST for Navigation Guidance category
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
                            _logger.LogDebug("Matched navigation keyword for Navigation Guidance, response selected");
                            break;
                        }
                    }
                }
            }
        }
        else if (category == "Status Inquiries")
        {
            // Check status FIRST for Status Inquiries category
            if (_knowledgeBase.Status != null && _knowledgeBase.Status.Any())
            {
                foreach (var item in _knowledgeBase.Status)
                {
                    if (item.Keywords != null && item.Keywords.Any(keyword => lowerMessage.Contains(keyword.ToLowerInvariant())))
                    {
                        if (item.Responses != null && item.Responses.Any())
                        {
                            var random = new Random();
                            response = item.Responses[random.Next(item.Responses.Count)];
                            _logger.LogDebug("Matched status keyword for Status Inquiries, response selected");
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            // Default order: navigation, services, status (when no category specified)
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
        }

        // Default response
        if (string.IsNullOrEmpty(response))
        {
            _logger.LogWarning("No keyword match found for message: {Message}", message);
            response = $"I understand you're asking about: \"{message}\". Could you please rephrase your question? I can help you with Absher navigation guidance, service explanations, or status inquiries.";
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
                    Keywords = new List<string> { "where", "find", "navigate", "section", "service", "menu", "how to find", "where is", "location", "page" },
                    Responses = new List<string>
                    {
                        "To find a service in Absher, you can use the main menu at the top of the page. Services are organized by categories like Civil Affairs, Traffic, Labor, and more. You can also use the search bar to quickly find what you're looking for.",
                        "In Absher, you can navigate to different services using the navigation menu. The main categories include: Civil Affairs (الأحوال المدنية), Traffic Services (المرور), Labor Services (العمل), and more. Click on any category to see available services.",
                        "To navigate Absher, use the top navigation menu. You'll find services grouped by type. You can also use the search feature by typing keywords related to the service you need. What specific service are you looking for?"
                    }
                }
            },
            Services = new List<KnowledgeItem>
            {
                new KnowledgeItem
                {
                    Keywords = new List<string> { "how", "explain", "what is", "what are", "how does", "how to", "steps", "process", "procedure", "guide" },
                    Responses = new List<string>
                    {
                        "I can explain how Absher services work! Each service has specific steps and requirements. Could you tell me which specific service you'd like me to explain? For example: ID renewal, passport application, work permit, etc.",
                        "I'm here to provide clear explanations of Absher services. Each service follows a specific process with required documents and steps. What service would you like me to explain?",
                        "I can walk you through how any Absher service works. Services typically require: logging in, selecting the service, providing required information, uploading documents if needed, and submitting. Which service are you interested in?"
                    }
                }
            },
            Status = new List<KnowledgeItem>
            {
                new KnowledgeItem
                {
                    Keywords = new List<string> { "status", "check", "track", "where is", "progress", "update", "application status", "request status" },
                    Responses = new List<string>
                    {
                        "To check your request status in Absher: 1) Log in to your Absher account, 2) Go to 'My Services' or 'My Requests' section, 3) Find your application/request, 4) Click on it to see detailed status. You can also receive SMS notifications about status updates.",
                        "You can check the status of any Absher request by logging in and going to the 'My Services' section. All your submitted applications and requests are listed there with their current status. Status updates are also sent via SMS.",
                        "To track your application: Log in to Absher → My Services/My Requests → Find your application → View status. Statuses typically include: Submitted, Under Review, Approved, Rejected, or Completed. You'll receive notifications for updates."
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

