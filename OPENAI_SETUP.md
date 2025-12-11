# OpenAI API Integration Setup

## Overview

The chatbot now supports OpenAI API integration for intelligent, context-aware responses. The system automatically falls back to the knowledge base if OpenAI is not configured or unavailable.

## Configuration

### 1. Get Your OpenAI API Key

1. Go to [https://platform.openai.com/](https://platform.openai.com/)
2. Sign up or log in to your account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the API key (you'll only see it once!)

### 2. Configure the API Key

#### Option A: appsettings.json (Development)

Edit `AIChatbot/appsettings.Development.json`:

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-api-key-here"
  }
}
```

#### Option B: appsettings.json (Production)

Edit `AIChatbot/appsettings.json`:

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-api-key-here"
  }
}
```

#### Option C: Environment Variables (Recommended for Production)

**Windows:**
```powershell
$env:OpenAI__ApiKey = "sk-your-api-key-here"
```

**Linux/Mac:**
```bash
export OpenAI__ApiKey="sk-your-api-key-here"
```

**In appsettings.json:**
```json
{
  "OpenAI": {
    "ApiKey": ""
  }
}
```

The empty string will allow the environment variable to override it.

#### Option D: User Secrets (Development - Most Secure)

```bash
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-api-key-here"
```

### 3. Restart the Application

After configuring the API key, restart your Blazor application:

```bash
dotnet run
```

## How It Works

### Fallback Mechanism

1. **OpenAI Available**: Uses GPT-3.5-turbo for intelligent, context-aware responses
2. **OpenAI Unavailable**: Automatically falls back to knowledge base responses
3. **No Configuration**: Works with knowledge base only (no errors)

### Response Flow

```
User Message
    ↓
ChatService.ProcessMessageAsync()
    ↓
Try OpenAI API
    ↓
Success? → Return AI Response
    ↓
Failed? → Fallback to Knowledge Base
    ↓
Return Response
```

## Features

### OpenAI Integration
- **Model**: GPT-3.5-turbo (cost-effective, fast)
- **Context**: Maintains conversation history (last 10 messages)
- **System Prompt**: Pre-configured with JMM Innovations context
- **Temperature**: 0.7 (balanced creativity and consistency)
- **Max Tokens**: 500 (concise responses)

### Knowledge Base Fallback
- Automatic fallback if OpenAI fails
- No user-visible errors
- Seamless experience

## Testing

### Test OpenAI Integration

1. Configure API key
2. Start the application
3. Open the chatbot
4. Ask a question that's not in the knowledge base
5. You should get an intelligent AI response

### Test Fallback

1. Remove or invalidate API key
2. Start the application
3. Open the chatbot
4. Ask a question
5. You should get a knowledge base response

## Cost Considerations

### OpenAI Pricing (as of 2024)
- **GPT-3.5-turbo**: ~$0.0015 per 1K input tokens, ~$0.002 per 1K output tokens
- **Average conversation**: ~500 tokens per exchange
- **Estimated cost**: ~$0.001 per conversation

### Cost Optimization Tips

1. **Max Tokens**: Set to 500 (already configured)
2. **Temperature**: 0.7 (balanced, not too creative)
3. **Model**: GPT-3.5-turbo (cheapest option)
4. **Context Window**: Limited to last 10 messages

## Troubleshooting

### OpenAI Not Working

**Check API Key:**
- Verify key is correct in configuration
- Ensure no extra spaces or quotes
- Check if key is active in OpenAI dashboard

**Check Logs:**
```bash
# Look for OpenAI-related log messages
dotnet run
```

**Common Errors:**
- `401 Unauthorized`: Invalid API key
- `429 Too Many Requests`: Rate limit exceeded
- `500 Internal Server Error`: OpenAI service issue

### Fallback to Knowledge Base

If OpenAI fails, the system automatically uses the knowledge base. Check logs to see why OpenAI failed:

```csharp
// In OpenAIService.cs, errors are logged
_logger.LogError(ex, "Error calling OpenAI API");
```

## Security Best Practices

1. **Never commit API keys to Git**
   - Use `.gitignore` for `appsettings.Development.json` if it contains keys
   - Use environment variables or user secrets

2. **Rotate API Keys Regularly**
   - Generate new keys periodically
   - Revoke old keys in OpenAI dashboard

3. **Monitor Usage**
   - Check OpenAI dashboard for usage
   - Set up billing alerts
   - Monitor costs regularly

4. **Limit Access**
   - Use API keys with minimal required permissions
   - Don't share keys publicly

## Advanced Configuration

### Custom System Prompt

Edit `AIChatbot/Services/OpenAIService.cs`:

```csharp
content = "Your custom system prompt here..."
```

### Change Model

Edit the request body in `OpenAIService.cs`:

```csharp
model = "gpt-4",  // More capable but more expensive
// or
model = "gpt-3.5-turbo",  // Current default
```

### Adjust Temperature

```csharp
temperature = 0.9,  // More creative
// or
temperature = 0.3,  // More deterministic
```

## Support

For issues:
- Check OpenAI API status: [https://status.openai.com/](https://status.openai.com/)
- Review OpenAI documentation: [https://platform.openai.com/docs](https://platform.openai.com/docs)
- Check application logs for detailed error messages

---

**Last Updated:** December 2024

