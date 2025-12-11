using System.Text;
using System.Text.RegularExpressions;

namespace AIChatbot.Services;

public class DocumentService
{
    private string? _documentContent;
    private string? _sourceUrl;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DocumentService> _logger;
    private readonly OpenAIService? _openAIService;
    private readonly HttpClient _httpClient;

    public DocumentService(
        IWebHostEnvironment environment,
        ILogger<DocumentService> logger,
        OpenAIService? openAIService,
        HttpClient httpClient)
    {
        _environment = environment;
        _logger = logger;
        _openAIService = openAIService;
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
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
        _sourceUrl = null;
        _logger.LogInformation("Document content loaded from text (length: {Length})", content.Length);
        await Task.CompletedTask;
    }

    public async Task<bool> LoadDocumentFromUrlAsync(string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URL cannot be empty");
            }

            // Ensure URL has a protocol
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            _logger.LogInformation("Fetching content from URL: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var htmlContent = await response.Content.ReadAsStringAsync();
            
            // Extract text from HTML
            var textContent = ExtractTextFromHtml(htmlContent);
            
            if (string.IsNullOrWhiteSpace(textContent))
            {
                throw new Exception("No text content could be extracted from the website");
            }

            _documentContent = textContent;
            _sourceUrl = url;
            
            _logger.LogInformation("Website content loaded successfully. Content length: {Length}", textContent.Length);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading document from URL: {Url}", url);
            throw;
        }
    }

    private string ExtractTextFromHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Remove script and style elements
        html = Regex.Replace(html, @"<script[^>]*>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<style[^>]*>[\s\S]*?</style>", "", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<noscript[^>]*>[\s\S]*?</noscript>", "", RegexOptions.IgnoreCase);

        // Remove HTML tags
        html = Regex.Replace(html, @"<[^>]+>", " ");

        // Decode HTML entities
        html = System.Net.WebUtility.HtmlDecode(html);

        // Clean up whitespace
        html = Regex.Replace(html, @"\s+", " ");
        html = Regex.Replace(html, @"\n\s*\n", "\n");

        // Limit content length to avoid token limits (keep first 15000 characters)
        if (html.Length > 15000)
        {
            html = html.Substring(0, 15000) + "... [Content truncated]";
        }

        return html.Trim();
    }

    public string? GetSourceUrl() => _sourceUrl;

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

Please extract ONLY information about '{category}' from the document above. 
Do NOT include information about other categories.

Focus specifically on:
- If category is 'Navigation Guidance': ONLY locations, addresses, directions, how to find/visit, office hours
- If category is 'Service Explanation': ONLY services offered, what they do, capabilities, offerings
- If category is 'Status Inquiries': ONLY how to check status, order tracking, request updates, processing times

Return ONLY a clear, concise summary of information related to '{category}'. Do not mention other categories.";

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
            return "No document or website has been loaded. Please load a document or website first.";
        }

        if (_openAIService == null || !_openAIService.IsAvailable)
        {
            return "OpenAI service is not available. Please configure the API key.";
        }

        try
        {
            var sourceInfo = !string.IsNullOrWhiteSpace(_sourceUrl) 
                ? $"Website URL: {_sourceUrl}\n" 
                : "Document Content:\n";

            var contextPrompt = $@"You are a helpful AI assistant. You answer questions based on the following content.

{sourceInfo}
{_documentContent}

Category: {category}
Current Question: {question}

Instructions:
- Answer the question based ONLY on the information in the content above
- If the content is from a website, focus on information from that website
- If the category is 'Website Content', answer based on the website content
- If the information is not in the content, politely say so
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

