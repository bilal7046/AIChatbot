namespace AIChatbot.Services;

public class ChatService
{
    private readonly OpenAIService? _openAIService;
    private readonly KnowledgeBaseService _knowledgeBaseService;
    private readonly DocumentService _documentService;
    private readonly OrderStatusService _orderStatusService;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        OpenAIService? openAIService,
        KnowledgeBaseService knowledgeBaseService,
        DocumentService documentService,
        OrderStatusService orderStatusService,
        ILogger<ChatService> logger)
    {
        _openAIService = openAIService;
        _knowledgeBaseService = knowledgeBaseService;
        _documentService = documentService;
        _orderStatusService = orderStatusService;
        _logger = logger;
    }

    public async Task<string> ProcessMessageAsync(string message, List<ChatMessage>? conversationHistory = null, string? category = null)
    {
        // Always check for ID number first (in case user provides it in follow-up message)
        var idNumber = _orderStatusService.ExtractIdNumber(message);
        if (!string.IsNullOrWhiteSpace(idNumber))
        {
            _logger.LogInformation("ID number extracted: {IdNumber}", idNumber);
            var status = _orderStatusService.GetOrderStatus(idNumber);
            var statusResponse = _orderStatusService.GetStatusResponse(idNumber, status);
            return statusResponse;
        }

        // Check for status inquiries in Status Inquiries category
        if (category == "Status Inquiries")
        {
            // Check if user is asking about status without providing ID number
            if (_orderStatusService.IsStatusInquiry(message))
            {
                // Check if we already asked for ID number in previous messages
                var alreadyAsked = conversationHistory != null && 
                    conversationHistory.Any(m => !m.IsUser && 
                    (m.Text.Contains("ID number", StringComparison.OrdinalIgnoreCase) || 
                     m.Text.Contains("national ID", StringComparison.OrdinalIgnoreCase) ||
                     m.Text.Contains("identity number", StringComparison.OrdinalIgnoreCase) ||
                     m.Text.Contains("provide your ID", StringComparison.OrdinalIgnoreCase)));
                
                if (!alreadyAsked)
                {
                    return "I'd be happy to check your application status! Could you please provide your ID number? " +
                           "You can find it on your ID card or Iqama.";
                }
            }
        }

        // If category is specified and document is loaded, use document-based answering
        if (!string.IsNullOrWhiteSpace(category) && _documentService.HasDocument)
        {
            return await _documentService.AnswerQuestionAsync(message, category, conversationHistory);
        }

        // Try OpenAI first if available
        if (_openAIService != null && _openAIService.IsAvailable)
        {
            try
            {
                // Pass category context to OpenAI if available
                var contextualMessage = !string.IsNullOrWhiteSpace(category) 
                    ? $"[Category: {category}] {message}"
                    : message;
                var response = await _openAIService.GetChatResponseAsync(contextualMessage, conversationHistory);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OpenAI service failed, falling back to knowledge base");
                // Fall back to knowledge base
            }
        }

        // Fallback to knowledge base - pass category to prioritize correct responses
        return _knowledgeBaseService.ProcessMessage(message, category);
    }

    public async Task<string> GetCategoryInformationAsync(string category)
    {
        if (_documentService.HasDocument)
        {
            return await _documentService.ExtractCategoryInformationAsync(category);
        }
        return "No document has been loaded. Please upload a document first.";
    }
}

