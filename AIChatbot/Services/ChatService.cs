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
        // Check for order number in status inquiries
        if (category == "Status Inquiries" || string.IsNullOrWhiteSpace(category))
        {
            var orderNumber = _orderStatusService.ExtractOrderNumber(message);
            if (!string.IsNullOrWhiteSpace(orderNumber))
            {
                _logger.LogInformation("Order number extracted: {OrderNumber}", orderNumber);
                var status = _orderStatusService.GetOrderStatus(orderNumber);
                var statusResponse = _orderStatusService.GetStatusResponse(orderNumber, status);
                return statusResponse;
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

        // Fallback to knowledge base
        return _knowledgeBaseService.ProcessMessage(message);
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

