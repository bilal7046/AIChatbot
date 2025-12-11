# JMM Innovations AI Chatbot Widget

A self-contained, floating web widget with text and voice input capabilities for multilingual virtual assistance.

## Overview

This MVP chatbot widget is designed to be easily embedded into any website. It supports:
- **Text input** - Type your questions directly
- **Voice input** - Press-to-speak functionality using Web Speech API
- **Smart responses** - Handles navigation, services, and status inquiries
- **Knowledge base** - Configurable responses from a JSON knowledge base

## Quick Start

### Option 1: Standalone Demo

1. Open `demo.html` in a web browser
2. The chatbot widget will appear in the bottom-right corner
3. Click the chat button to start interacting

### Option 2: Embed in Your Website

1. Copy `widget/chatbot-widget.js` to your project
2. Copy `widget/knowledge-base.json` to your project (or customize it)
3. Add the following to your HTML:

```html
<!-- At the end of your HTML, before </body> -->
<script src="path/to/chatbot-widget.js"></script>
<script>
    window.JMMChatbot = new ChatbotWidget({
        position: 'bottom-right',  // or 'bottom-left', 'top-right', 'top-left'
        primaryColor: '#007bff',    // Your brand color
        knowledgeBase: 'path/to/knowledge-base.json'  // Optional, uses default if not provided
    });
</script>
```

## File Structure

```
├── widget/
│   ├── chatbot-widget.js      # Main widget JavaScript file
│   └── knowledge-base.json    # Knowledge base with responses
├── demo.html                   # Demo webpage
└── README.md                   # This file
```

## Customization

### Updating the Knowledge Base

Edit `widget/knowledge-base.json` to customize responses. The structure is:

```json
{
  "navigation": [
    {
      "keywords": ["where", "location", "find"],
      "responses": [
        "Response 1",
        "Response 2"
      ]
    }
  ],
  "services": [...],
  "status": [...]
}
```

**How to add/edit responses:**
1. Open `widget/knowledge-base.json`
2. Find the relevant category (navigation, services, or status)
3. Add keywords that should trigger the response
4. Add or modify response strings
5. Save the file
6. Reload your webpage

### Changing Widget Position

When initializing the widget, set the `position` option:
- `'bottom-right'` (default)
- `'bottom-left'`
- `'top-right'`
- `'top-left'`

### Changing Colors

Set the `primaryColor` option when initializing:
```javascript
window.JMMChatbot = new ChatbotWidget({
    primaryColor: '#your-color-here'
});
```

## Browser Compatibility

- ✅ Chrome/Edge (full support including voice)
- ✅ Safari (full support including voice)
- ✅ Firefox (text input works, voice may have limited support)
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)

**Note:** Voice input requires browser support for Web Speech API. Chrome and Safari have the best support.

## Deployment

### Static Hosting

This widget can be deployed to any static hosting service:

1. **Netlify:**
   - Drag and drop the folder
   - Or connect via Git

2. **Vercel:**
   - Install Vercel CLI: `npm i -g vercel`
   - Run: `vercel`

3. **GitHub Pages:**
   - Push to GitHub
   - Enable GitHub Pages in repository settings

4. **Any Web Server:**
   - Upload all files to your web server
   - Ensure `demo.html` and the `widget/` folder are in the same directory structure

### Testing Locally

1. Use a local web server (required for JSON loading):
   ```bash
   # Python 3
   python -m http.server 8000
   
   # Node.js (with http-server)
   npx http-server
   
   # PHP
   php -S localhost:8000
   ```

2. Open `http://localhost:8000/demo.html` in your browser

## Technical Details

### Architecture

- **Pure JavaScript** - No dependencies, no frameworks
- **Web Speech API** - Browser-native speech recognition
- **JSON Knowledge Base** - Easy to update and maintain
- **CSS-in-JS** - Self-contained styling

### How It Works

1. Widget initializes and creates floating UI elements
2. User interacts via text or voice input
3. Voice input uses Web Speech API to convert speech to text
4. Text is matched against knowledge base keywords
5. Appropriate response is selected and displayed
6. All processing happens client-side (no backend required)

### Response Matching

The widget uses keyword matching:
- User message is converted to lowercase
- Keywords from knowledge base are checked
- First matching category determines the response
- Random response is selected from available responses in that category

## Troubleshooting

### Voice Input Not Working

1. **Check browser support:** Chrome and Safari have the best support
2. **Check permissions:** Browser may need microphone permission
3. **Check HTTPS:** Some browsers require HTTPS for microphone access
4. **Check console:** Open browser DevTools to see any errors

### Knowledge Base Not Loading

1. **Check file path:** Ensure the path to `knowledge-base.json` is correct
2. **Check CORS:** If loading from different domain, ensure CORS headers are set
3. **Check file format:** Ensure JSON is valid (use a JSON validator)

### Widget Not Appearing

1. **Check JavaScript errors:** Open browser console (F12)
2. **Check script loading:** Ensure `chatbot-widget.js` is loaded
3. **Check initialization:** Ensure `ChatbotWidget` is instantiated

## Future Enhancements

Potential improvements for full-scale version:
- Multi-language support
- Backend integration for dynamic responses
- User authentication
- Chat history
- Analytics
- Custom AI model integration
- File upload support

## Support

For questions or issues, contact: partner@jmminnovations.com

## License

Full IP ownership transferred to JMM Innovations upon payment.

---

**Version:** 1.0.0  
**Last Updated:** December 2024

