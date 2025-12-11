# Blazor Chatbot Widget Implementation Guide

## Overview

The chatbot widget has been successfully converted to a Blazor Server component. This implementation provides the same functionality as the JavaScript version but with the benefits of server-side rendering and .NET integration.

## Project Structure

```
AIChatbot/
├── Components/
│   └── Chatbot/
│       ├── ChatbotWidget.razor          # Main chatbot component
│       └── ChatbotWidget.razor.css       # Component styles
├── Services/
│   └── KnowledgeBaseService.cs           # Knowledge base logic
├── wwwroot/
│   ├── js/
│   │   └── speech-recognition.js         # JavaScript interop for voice
│   └── knowledge-base.json               # Knowledge base data
└── Components/
    └── Pages/
        └── Home.razor                    # Demo page with chatbot
```

## Key Components

### 1. ChatbotWidget.razor

The main Blazor component that renders the chatbot UI. Features:
- **Parameters:**
  - `Position` - Widget position (bottom-right, bottom-left, top-right, top-left)
  - `PrimaryColor` - Brand color (hex format, e.g., "#007bff")
- **Functionality:**
  - Text input handling
  - Voice input via JavaScript interop
  - Message display
  - Chat open/close toggle

### 2. KnowledgeBaseService.cs

Service that handles:
- Loading knowledge base from JSON file
- Processing user messages
- Matching keywords to categories
- Returning appropriate responses

**Registration:** Registered as Singleton in `Program.cs`

### 3. speech-recognition.js

JavaScript module for Web Speech API integration:
- Initializes speech recognition
- Handles voice input events
- Communicates with Blazor via JSInterop

## Usage

### Basic Usage

Add the component to any Blazor page:

```razor
<ChatbotWidget Position="bottom-right" PrimaryColor="#007bff" />
```

### Customization

```razor
<ChatbotWidget 
    Position="bottom-left" 
    PrimaryColor="#28a745" />
```

### Parameters

- **Position**: `"bottom-right"` (default), `"bottom-left"`, `"top-right"`, `"top-left"`
- **PrimaryColor**: Any valid hex color (e.g., `"#007bff"`)

## How It Works

### Message Flow

1. User types or speaks a message
2. Component receives input
3. `KnowledgeBaseService.ProcessMessage()` is called
4. Service matches keywords against knowledge base
5. Response is selected and returned
6. Component displays response

### Voice Input Flow

1. User presses and holds microphone button
2. JavaScript `speech-recognition.js` starts Web Speech API
3. Speech is converted to text
4. Text is sent to Blazor via JSInterop
5. `OnSpeechResult` method receives transcript
6. Message is processed like text input

## Knowledge Base Management

### Location
`wwwroot/knowledge-base.json`

### Structure
```json
{
  "navigation": [
    {
      "keywords": ["where", "location", "find"],
      "responses": ["Response 1", "Response 2"]
    }
  ],
  "services": [...],
  "status": [...]
}
```

### Updating Responses

1. Edit `wwwroot/knowledge-base.json`
2. Restart the application (or reload knowledge base)
3. Changes take effect immediately

**Note:** The service loads the knowledge base on application startup. For hot-reload during development, you may need to restart the app.

## Service Registration

The `KnowledgeBaseService` is registered in `Program.cs`:

```csharp
builder.Services.AddSingleton<KnowledgeBaseService>();

// Load knowledge base on startup
var knowledgeBaseService = app.Services.GetRequiredService<KnowledgeBaseService>();
await knowledgeBaseService.LoadKnowledgeBaseAsync();
```

## JavaScript Interop

The component uses JavaScript interop for:
- **Speech Recognition**: Web Speech API access
- **Scrolling**: Auto-scroll to bottom of messages

### JSInterop Methods

**From Blazor to JavaScript:**
- `initialize()` - Initialize speech recognition
- `startListening()` - Start voice input
- `stopListening()` - Stop voice input
- `scrollToBottom()` - Scroll messages container

**From JavaScript to Blazor:**
- `OnSpeechResult(string transcript)` - Receive voice transcript
- `OnSpeechError(string error)` - Handle speech errors

## Styling

Styles are defined in `ChatbotWidget.razor.css` using CSS isolation. The component uses:
- Scoped CSS (component-specific)
- Inline styles for dynamic colors (PrimaryColor parameter)
- Responsive design for mobile devices

## Browser Compatibility

- ✅ **Chrome/Edge**: Full support (text + voice)
- ✅ **Safari**: Full support (text + voice)
- ⚠️ **Firefox**: Text input works, voice may have limited support
- ✅ **Mobile**: iOS Safari and Chrome Mobile supported

## Advantages of Blazor Implementation

1. **Server-Side Processing**: All logic runs on server
2. **Type Safety**: C# instead of JavaScript
3. **Easy Integration**: Works with existing .NET services
4. **Maintainability**: Easier to maintain and extend
5. **Security**: Server-side validation and processing

## Deployment

### Development
```bash
dotnet run
```

### Production
```bash
dotnet publish -c Release
```

Deploy to:
- Azure App Service
- IIS
- Docker containers
- Any .NET hosting platform

## Troubleshooting

### Voice Input Not Working

1. **Check browser support**: Chrome/Safari recommended
2. **Check permissions**: Browser needs microphone access
3. **Check HTTPS**: Some browsers require HTTPS for microphone
4. **Check console**: Look for JavaScript errors in browser DevTools

### Knowledge Base Not Loading

1. **Check file path**: Ensure `knowledge-base.json` is in `wwwroot/`
2. **Check JSON format**: Validate JSON syntax
3. **Check logs**: Check application logs for loading errors

### Component Not Rendering

1. **Check imports**: Ensure `_Imports.razor` includes Chatbot namespace
2. **Check service registration**: Verify `KnowledgeBaseService` is registered
3. **Check browser console**: Look for JavaScript errors

## Next Steps

### Potential Enhancements

1. **Multi-language Support**: Add language detection and translation
2. **Backend Integration**: Connect to real APIs for status checks
3. **User Authentication**: Add user context for personalized responses
4. **Chat History**: Store conversation history
5. **Analytics**: Track user interactions
6. **AI Integration**: Connect to OpenAI or Azure OpenAI
7. **File Upload**: Support file attachments

## Comparison: JavaScript vs Blazor

| Feature | JavaScript Version | Blazor Version |
|---------|-------------------|----------------|
| **Deployment** | Static hosting | .NET hosting |
| **Processing** | Client-side | Server-side |
| **Language** | JavaScript | C# |
| **Type Safety** | No | Yes |
| **Integration** | Standalone | .NET ecosystem |
| **Maintenance** | Manual | IDE support |

Both versions provide the same user experience, but the Blazor version offers better integration with .NET applications and server-side processing capabilities.

---

**Version:** 1.0.0  
**Last Updated:** December 2024

