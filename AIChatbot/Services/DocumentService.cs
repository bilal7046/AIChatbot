using System.Text;

namespace AIChatbot.Services;

public class DocumentService
{
    private string? _documentContent;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DocumentService> _logger;
    private readonly OpenAIService? _openAIService;

    public DocumentService(
        IWebHostEnvironment environment,
        ILogger<DocumentService> logger,
        OpenAIService? openAIService)
    {
        _environment = environment;
        _logger = logger;
        _openAIService = openAIService;
    }

    public bool HasDocument => !string.IsNullOrWhiteSpace(_documentContent);

    public async Task LoadDocumentFromFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);
            if (File.Exists(fullPath))
            {
                _documentContent = await File.ReadAllTextAsync(fullPath);
                _logger.LogInformation("Document loaded from file: {FilePath}", filePath);
            }
            else
            {
                _logger.LogWarning("Document file not found: {FilePath}", fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading document from file");
            throw;
        }
    }

    public async Task LoadDocumentFromTextAsync(string content)
    {
        _documentContent = content;
        _logger.LogInformation("Document content loaded from text (length: {Length})", content.Length);
        await Task.CompletedTask;
    }

    public async Task<string> ExtractCategoryInformationAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(_documentContent))
        {
            return "No document has been loaded. Please upload a document first.";
        }

        if (_openAIService == null || !_openAIService.IsAvailable)
        {
            // Fallback: return raw document content if OpenAI is not available
            return _documentContent;
        }

        try
        {
            var prompt = $@"You are an AI assistant that extracts information from documents.

Document Content:
{_documentContent}

Category: {category}

Please extract and summarize all relevant information about '{category}' from the document above. 
Focus on:
- Navigation Guidance: locations, addresses, directions, how to find/visit
- Service Explanation: services offered, what they do, capabilities, offerings
- Status Inquiries: how to check status, order tracking, request updates, processing times

Return a clear, concise summary of information related to this category. If no relevant information is found, say so.";

            var response = await _openAIService.GetChatResponseAsync(prompt, null);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting category information");
            return $"Error extracting information: {ex.Message}";
        }
    }

    public string GetDocumentContent()
    {
        return _documentContent ?? string.Empty;
    }

    public async Task<string> AnswerQuestionAsync(string question, string category, List<ChatMessage>? conversationHistory = null)
    {
        if (string.IsNullOrWhiteSpace(_documentContent))
        {
            return "No document has been loaded. Please upload a document first.";
        }

        if (_openAIService == null || !_openAIService.IsAvailable)
        {
            return "OpenAI service is not available. Please configure the API key.";
        }

        try
        {
            var contextPrompt = $@"You are a helpful AI assistant for a business. You answer questions based on the following document.

Document Content:
{_documentContent}

Category: {category}
Current Question: {question}

Instructions:
- Answer the question based ONLY on the information in the document above
- Focus on the '{category}' category
- If the information is not in the document, politely say so
- Be concise, friendly, and professional
- If asked about navigation, provide location details, directions, addresses
- If asked about services, explain what services are offered
- If asked about status, explain how to check status, track orders, or get updates";

            var response = await _openAIService.GetChatResponseAsync(contextPrompt, conversationHistory);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error answering question");
            return $"I apologize, but I encountered an error: {ex.Message}";
        }
    }
}

