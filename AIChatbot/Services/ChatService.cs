namespace AIChatbot.Services;

public class ChatService
{
    private readonly OpenAIService? _openAIService;
    private readonly KnowledgeBaseService _knowledgeBaseService;
    private readonly DocumentService _documentService;
    private readonly ILogger<ChatService> _logger;

    public ChatService(
        OpenAIService? openAIService,
        KnowledgeBaseService knowledgeBaseService,
        DocumentService documentService,
        ILogger<ChatService> logger)
    {
        _openAIService = openAIService;
        _knowledgeBaseService = knowledgeBaseService;
        _documentService = documentService;
        _logger = logger;
    }

    public async Task<string> ProcessMessageAsync(string message, List<ChatMessage>? conversationHistory = null, string? category = null)
    {
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
                var response = await _openAIService.GetChatResponseAsync(message, conversationHistory);
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

